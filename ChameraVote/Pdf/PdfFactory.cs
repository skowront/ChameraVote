using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PdfSharp;
using MigraDoc;
using ChameraVote.ViewModels;
using PdfSharp.Drawing;
using System.Globalization;
using PdfSharp.Pdf;
using MigraDoc.DocumentObjectModel;
using MigraDoc.DocumentObjectModel.Tables;

namespace ChameraVote.Pdf
{
    public class PdfFactory
    {
        public PdfFactory()
        {

        }

        void DefineStyles(Document document)
        {
            // Get the predefined style Normal.
            Style style = document.Styles["Normal"];
            // Because all styles are derived from Normal, the next line changes the 
            // font of the whole document. Or, more exactly, it changes the font of
            // all styles and paragraphs that do not redefine the font.
            style.Font.Name = "Verdana";

            style = document.Styles[StyleNames.Header];
            style.ParagraphFormat.AddTabStop("16cm", TabAlignment.Right);

            style = document.Styles[StyleNames.Footer];
            style.ParagraphFormat.AddTabStop("8cm", TabAlignment.Center);

            // Create a new style called Table based on style Normal
            style = document.Styles.AddStyle("Table", "Normal");
            style.Font.Name = "Verdana";
            style.Font.Name = "Times New Roman";
            style.Font.Size = 9;

            // Create a new style called Reference based on style Normal
            style = document.Styles.AddStyle("Reference", "Normal");
            style.ParagraphFormat.SpaceBefore = "5mm";
            style.ParagraphFormat.SpaceAfter = "5mm";
            style.ParagraphFormat.TabStops.AddTabStop("16cm", TabAlignment.Right);

        }

        public string GetDate()
        {
            DateTimeFormatInfo info = CultureInfo.CurrentCulture.DateTimeFormat;
            return DateTime.Now.ToString(info);
        }

        public Table BuildVoteTable(VotingSumResultsViewModel votingSumResultsViewModel, Table table)
        {
            Column column = table.AddColumn();
            column.Format.Alignment = ParagraphAlignment.Center;

            foreach (var option in votingSumResultsViewModel.VotingResultsViewModel.VotingViewModel.VotingOptionsRaw)
            {
                column = table.AddColumn();
                column.Format.Alignment = ParagraphAlignment.Center;
            }

            var row = table.AddRow();

            for (int i = 1; i<table.Columns.Count;i++)
            {
                var p = row.Cells[i].AddParagraph();
                p.AddText(votingSumResultsViewModel.VotingResultsViewModel.VotingViewModel.VotingOptionsRaw[i-1]);
            }

            foreach(var card in votingSumResultsViewModel.VotingResultsViewModel.Votes)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(card.Username);
                for (int i = 1; i < table.Columns.Count; i++)
                {
                    var option = votingSumResultsViewModel.VotingResultsViewModel.VotingViewModel.VotingOptionsRaw[i - 1];
                    if (card.Options.Contains(option))
                    {
                        row.Cells[i].AddParagraph("+");
                    }
                    else
                    {
                        row.Cells[i].AddParagraph("-");
                    }
                }
            }

            row = table.AddRow();
            for (int i = 1; i < table.Columns.Count; i++)
            {
                var p = row.Cells[i].AddParagraph();
                p.AddText(votingSumResultsViewModel.VotingOptionSums[i - 1].Votes.ToString());
            }

            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;

            return table;
        }

        public Document BuildVotingResultsPdfPL(VotingSumResultsViewModel votingSumResultsViewModel)
        {
            Document document = new Document();
            this.DefineStyles(document);
            
            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            var text = paragraph.AddText("Głosowanie:");
            text = paragraph.AddText(votingSumResultsViewModel.VotingResultsViewModel.VotingViewModel.VotingTitle);

            paragraph = section.AddParagraph(this.GetDate());
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            var table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Rows.LeftIndent = 0;
            table.Format.Font.Size = 15;

            Column column = table.AddColumn("10cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.RoyalBlue;
            row.Cells[0].AddParagraph("Kategoria");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("Liczba głosów");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;

            foreach (var item in votingSumResultsViewModel.VotingOptionSums)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(item.OptionValue);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                row.Cells[1].AddParagraph(item.Votes.ToString());
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            }

            foreach (var votingCard in votingSumResultsViewModel.VotingResultsViewModel.Votes)
            {
                paragraph = section.AddParagraph();
                paragraph.Format.Font.Size = 15;
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.AddText("\n");
                paragraph.AddText("\n");

                table = section.AddTable();
                table.Style = "Table";
                table.Borders.Color = Colors.Black;
                table.Borders.Width = 0.25;
                table.Rows.LeftIndent = 0;
                table.Format.Font.Size = 15;

                column = table.AddColumn("10cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Shading.Color = Colors.RoyalBlue;
                row.Cells[0].AddParagraph("Karta do głosowania");
                row.Cells[0].AddParagraph(votingCard.Username);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

                foreach(var option in votingCard.Options)
                {
                    row = table.AddRow();
                    row.Cells[0].AddParagraph(option);
                    row.Cells[0].Format.Font.Bold = false;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                }
            }

            paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            table = this.BuildVoteTable(votingSumResultsViewModel,section.AddTable());

            paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Rows.LeftIndent = 0;
            table.Format.Font.Size = 15;

            column = table.AddColumn("10cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.RoyalBlue;
            row.Cells[0].AddParagraph("Głosujący");

            foreach(var item in votingSumResultsViewModel.VotingClients)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(item);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            }

            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddPageField();
            section.Footers.EvenPage.Add(paragraph.Clone());

            return document;
        }

        public Document BuildVotingResultsPdfEN(VotingSumResultsViewModel votingSumResultsViewModel)
        {
            Document document = new Document();
            this.DefineStyles(document);

            Section section = document.AddSection();
            Paragraph paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 20;
            paragraph.Format.Alignment = ParagraphAlignment.Center;

            var text = paragraph.AddText("Voting:");
            text = paragraph.AddText(votingSumResultsViewModel.VotingResultsViewModel.VotingViewModel.VotingTitle);

            paragraph = section.AddParagraph(this.GetDate());
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            var table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Rows.LeftIndent = 0;
            table.Format.Font.Size = 15;

            Column column = table.AddColumn("10cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            column = table.AddColumn("5cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            Row row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.RoyalBlue;
            row.Cells[0].AddParagraph("Category");
            row.Cells[0].Format.Font.Bold = false;
            row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            row.Cells[1].AddParagraph("Number of votes");
            row.Cells[1].Format.Font.Bold = false;
            row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
            row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;

            foreach (var item in votingSumResultsViewModel.VotingOptionSums)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(item.OptionValue);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                row.Cells[1].AddParagraph(item.Votes.ToString());
                row.Cells[1].Format.Font.Bold = false;
                row.Cells[1].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[1].VerticalAlignment = VerticalAlignment.Bottom;
            }

            foreach (var votingCard in votingSumResultsViewModel.VotingResultsViewModel.Votes)
            {
                paragraph = section.AddParagraph();
                paragraph.Format.Font.Size = 15;
                paragraph.Format.Alignment = ParagraphAlignment.Center;
                paragraph.AddText("\n");
                paragraph.AddText("\n");

                table = section.AddTable();
                table.Style = "Table";
                table.Borders.Color = Colors.Black;
                table.Borders.Width = 0.25;
                table.Rows.LeftIndent = 0;
                table.Format.Font.Size = 15;

                column = table.AddColumn("10cm");
                column.Format.Alignment = ParagraphAlignment.Center;

                row = table.AddRow();
                row.Shading.Color = Colors.RoyalBlue;
                row.Cells[0].AddParagraph("Ballot");
                row.Cells[0].AddParagraph(votingCard.Username);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;

                foreach (var option in votingCard.Options)
                {
                    row = table.AddRow();
                    row.Cells[0].AddParagraph(option);
                    row.Cells[0].Format.Font.Bold = false;
                    row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                    row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
                }
            }

            paragraph = section.AddParagraph(this.GetDate());
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            table = this.BuildVoteTable(votingSumResultsViewModel, section.AddTable());

            paragraph = section.AddParagraph();
            paragraph.Format.Font.Size = 15;
            paragraph.Format.Alignment = ParagraphAlignment.Center;
            paragraph.AddText("\n");
            paragraph.AddText("\n");

            table = section.AddTable();
            table.Style = "Table";
            table.Borders.Color = Colors.Black;
            table.Borders.Width = 0.25;
            table.Rows.LeftIndent = 0;
            table.Format.Font.Size = 15;

            column = table.AddColumn("10cm");
            column.Format.Alignment = ParagraphAlignment.Center;

            row = table.AddRow();
            row.HeadingFormat = true;
            row.Format.Alignment = ParagraphAlignment.Center;
            row.Format.Font.Bold = true;
            row.Shading.Color = Colors.RoyalBlue;
            row.Cells[0].AddParagraph("Voters");


            foreach (var item in votingSumResultsViewModel.VotingClients)
            {
                row = table.AddRow();
                row.Cells[0].AddParagraph(item);
                row.Cells[0].Format.Font.Bold = false;
                row.Cells[0].Format.Alignment = ParagraphAlignment.Left;
                row.Cells[0].VerticalAlignment = VerticalAlignment.Bottom;
            }

            paragraph = section.Footers.Primary.AddParagraph();
            paragraph.AddPageField();
            section.Footers.EvenPage.Add(paragraph.Clone());

            return document;
        }

        public Document BuildVotingResultsPdf(VotingSumResultsViewModel votingSumResultsViewModel,string language="")
        {
            switch(language)
            {
                case "PL":
                    return this.BuildVotingResultsPdfPL(votingSumResultsViewModel);
                case "EN":
                    return this.BuildVotingResultsPdfEN(votingSumResultsViewModel);
                default:
                    return this.BuildVotingResultsPdfEN(votingSumResultsViewModel);
            }
        }
    }
}
