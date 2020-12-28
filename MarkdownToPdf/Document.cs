using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Microsoft.Toolkit.Parsers.Markdown;
using Microsoft.Toolkit.Parsers.Markdown.Blocks;
using Microsoft.Toolkit.Parsers.Markdown.Inlines;

namespace MarkdownToPdf
{
    [DebuggerDisplay("{" + nameof(Name) + "}")]
    public class Document
    {
        public string Name { get; private set; }
        private readonly string _fileName;
        private readonly MarkdownDocument _markdown;
        
        public Document(string fileName)
        {
            _fileName = fileName;
            Name = Path.GetFileNameWithoutExtension(fileName);
            
            var markdownText = File.ReadAllText(fileName);
            _markdown = new MarkdownDocument();
            _markdown.Parse(markdownText);
        }

        public List<MarkdownBlock> GetMarkdownBlocks() => _markdown.Blocks.ToList();
        
        public List<string> GetReferencedDocuments()
        {
            var referencedDocuments = new List<string>();
            
            foreach (var block in _markdown.Blocks)
            {
                if (!(block is ParagraphBlock paragraph)) continue;
                
                foreach (var inline in paragraph.Inlines)
                {
                    if (!(inline is MarkdownLinkInline mdLink)) continue;
                    
                    var url = mdLink.Url;
                    if (url == null)
                    {
                        Console.WriteLine($"{_fileName}: Link without URL {mdLink.ReferenceId}");
                        continue;
                    }
                    referencedDocuments.Add(url);
                }
            }

            return referencedDocuments;
        }

    }
}