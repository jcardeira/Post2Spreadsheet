using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Google.GData.Documents;
using Google.GData.Client;
using Google.GData.Spreadsheets;
using System.Collections.Specialized;

namespace cardeira.p2s
{
    class GDataAPI
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public GDataAPI(NameValueCollection parameters)
        {
            try
            {
                SpreadsheetsService service = new SpreadsheetsService("post2spreadsheet");
                service.setUserCredentials(cardeira.Properties.Settings.Default.gUserName, cardeira.Properties.Settings.Default.gPassword);

                Google.GData.Spreadsheets.SpreadsheetQuery query = new Google.GData.Spreadsheets.SpreadsheetQuery();

                SpreadsheetFeed feed = service.Query(query);
                SpreadsheetEntry entry = null;
                foreach (SpreadsheetEntry e in feed.Entries)
                {
                    entry = e;
                    logger.Debug("Spreadsheet: {0}", entry.Title.Text);
                    if (entry.Title.Text == cardeira.Properties.Settings.Default.spreadsheetkey)
                        break;
                }
                if (entry != null)
                    InsertRow(service, (WorksheetEntry)entry.Worksheets.Entries[0], parameters);
            }
            catch (Exception e)
            {
                logger.ErrorException("error writing to spreadsheet", e);
            }
        }

        private static ListEntry InsertRow(SpreadsheetsService service, WorksheetEntry entry, NameValueCollection parameters)
        {
            logger.Debug("inserting row...");
            AtomLink listFeedLink = entry.Links.FindService(GDataSpreadsheetsNameTable.ListRel, null);

            ListQuery query = new ListQuery(listFeedLink.HRef.ToString());
            
            ListFeed feed = service.Query(query);
            
            ListEntry newRow = new ListEntry();
            foreach(string key in parameters)
            {
                ListEntry.Custom curElement = new ListEntry.Custom();
                curElement.Value = parameters[key];
                curElement.LocalName = key;
                newRow.Elements.Add(curElement);
            }

            // add datetime
            ListEntry.Custom el = new ListEntry.Custom();
            el.Value = parameters["data"];
            el.LocalName = DateTime.Now.ToString() ;
            newRow.Elements.Add(el);

            ListEntry insertedRow = feed.Insert(newRow);
            return insertedRow;
        }

    }



}