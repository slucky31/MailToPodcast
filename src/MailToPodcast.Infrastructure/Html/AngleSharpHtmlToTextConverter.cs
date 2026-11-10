using System.Text;
using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using MailToPodcast.Core.Mails;

namespace MailToPodcast.Infrastructure.Html;

/// <summary>
/// Converts a newsletter's HTML into plain text: drops what is not read aloud (scripts, styles, images,
/// hidden blocks, unsubscribe and "view in browser" lines) and keeps paragraphs, headings and list items.
/// </summary>
internal sealed partial class AngleSharpHtmlToTextConverter : IHtmlToTextConverter
{
    private static readonly HashSet<string> IgnoredTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "script", "style", "head", "noscript", "img", "svg", "iframe", "object", "template", "button", "input", "select",
    };

    private static readonly HashSet<string> BlockTags = new(StringComparer.OrdinalIgnoreCase)
    {
        "p", "div", "section", "article", "header", "footer", "aside", "main", "nav", "blockquote", "pre",
        "h1", "h2", "h3", "h4", "h5", "h6", "ul", "ol", "li", "table", "tr", "td", "th", "hr", "figure", "figcaption",
    };

    public string Convert(string html)
    {
        ArgumentNullException.ThrowIfNull(html);

        var document = new HtmlParser().ParseDocument(html);
        var builder = new StringBuilder();
        if (document.Body is not null)
        {
            AppendNode(document.Body, builder);
        }

        return Normalize(builder.ToString());
    }

    private static void AppendNode(INode node, StringBuilder builder)
    {
        foreach (var child in node.ChildNodes)
        {
            switch (child)
            {
                case IText text:
                    builder.Append(text.Data);
                    break;
                case IElement element:
                    AppendElement(element, builder);
                    break;
                default:
                    break;
            }
        }
    }

    private static void AppendElement(IElement element, StringBuilder builder)
    {
        if (IgnoredTags.Contains(element.LocalName) || IsHidden(element))
        {
            return;
        }

        if (element.LocalName == "br")
        {
            builder.Append('\n');
            return;
        }

        var isBlock = BlockTags.Contains(element.LocalName);
        if (isBlock)
        {
            builder.Append("\n\n");
        }

        if (element.LocalName == "li")
        {
            builder.Append("- ");
        }

        AppendNode(element, builder);

        if (isBlock)
        {
            builder.Append("\n\n");
        }
    }

    private static bool IsHidden(IElement element)
    {
        if (element.HasAttribute("hidden") || element.GetAttribute("aria-hidden") == "true")
        {
            return true;
        }

        var style = element.GetAttribute("style");
        return style is not null && HiddenStyleRegex().IsMatch(style);
    }

    private static string Normalize(string text)
    {
        var paragraphs = text
            .Replace("\r", string.Empty, StringComparison.Ordinal)
            .Split("\n\n", StringSplitOptions.RemoveEmptyEntries)
            .Select(paragraph => string.Join('\n', paragraph
                .Split('\n')
                .Select(line => SpacesRegex().Replace(InvisibleRegex().Replace(line, string.Empty).Replace('\u00A0', ' '), " ").Trim())
                .Where(line => line.Length > 0 && line != "-")))
            .Where(paragraph => paragraph.Length > 0 && !BoilerplateRegex().IsMatch(paragraph));

        return string.Join("\n\n", paragraphs);
    }

    [GeneratedRegex(@"display\s*:\s*none|visibility\s*:\s*hidden|max-height\s*:\s*0", RegexOptions.IgnoreCase)]
    private static partial Regex HiddenStyleRegex();

    [GeneratedRegex(@"[\u00AD\u034F\u200B-\u200F\u2060\uFEFF]")]
    private static partial Regex InvisibleRegex();

    [GeneratedRegex(@"[ \t\f\v]+")]
    private static partial Regex SpacesRegex();

    [GeneratedRegex(
        @"unsubscribe|se d[ée]sinscrire|d[ée]sabonner|d[ée]sinscription|view (this )?(e-?mail |it |post )?(online|in (your |a )?browser)|read (it )?(online|in (the )?app)|voir (la version )?en ligne|version en ligne|manage (your )?preferences|g[ée]rer (vos|mes) pr[ée]f[ée]rences",
        RegexOptions.IgnoreCase)]
    private static partial Regex BoilerplateRegex();
}
