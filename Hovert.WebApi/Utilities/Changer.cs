using Aspose.Words;
using Aspose.Words.Fields;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WEBAPIODATAV3.Utilities
{
   
    class FontChanger : DocumentVisitor
    {        

        public override VisitorAction VisitFieldEnd(FieldEnd fieldEnd)
        {

            //Simply change font name
            ResetFont(fieldEnd.Font);
            return VisitorAction.Continue;

        }


        

        public override VisitorAction VisitFieldSeparator(FieldSeparator fieldSeparator)
        {

            ResetFont(fieldSeparator.Font);
            return VisitorAction.Continue;

        }


       

        public override VisitorAction VisitFieldStart(FieldStart fieldStart)
        {

            ResetFont(fieldStart.Font);
            return VisitorAction.Continue;

        }




        public override VisitorAction VisitFootnoteEnd(Footnote footnote)
        {

            ResetFont(footnote.Font);
            return VisitorAction.Continue;

        }




        public override VisitorAction VisitFormField(FormField formField)
        {

            ResetFont(formField.Font);
            return VisitorAction.Continue;

        }




        public override VisitorAction VisitParagraphEnd(Paragraph paragraph)
        {

            ResetFont(paragraph.ParagraphBreakFont);

            return VisitorAction.Continue;

        }




        public override VisitorAction VisitRun(Run run)
        {

            ResetFont(run.Font);

            return VisitorAction.Continue;

        }




        public override VisitorAction VisitSpecialChar(SpecialChar specialChar)
        {

            ResetFont(specialChar.Font);

            return VisitorAction.Continue;

        }


        private void ResetFont(Aspose.Words.Font font)
        {

            font.Name = mNewFont;

        }




        private string mNewFont = "David";  // default

    }
}