using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Web_Forum_cSharp.Models;

namespace Web_Forum_cSharp.Controllers
{
    public class SearchController : Controller
    {
        private ThreadEntities db = new ThreadEntities();

        private String keyword = ""; 

        // GET: /Search/
        public ActionResult Index(String keyword)
        {
            if (keyword == null || keyword == "")
                return RedirectToAction("Index", "Home");

            this.keyword = keyword;

            List<Thread> threads = getMatchingThread();

            if (threads.Count == 0)
                return RedirectToAction("Index", "Home");
            else
                return View(threads);
        }


        private List<Thread> getMatchingThread()
        {

            //might have to return a dbset in if not work like expected

            List<Thread> titles = getMatchingTitle();
            List<Thread> content = getMatchingContent();
            List<Thread> tags = getMatchingTags();

            List<Thread> threads = titles
                                    .Concat(content)
                                    .Concat(tags).Distinct().Take(20).ToList<Thread>();

            return threads; 
        }


        //search title
        private List<Thread> getMatchingTitle()
        {
            List<Thread> matchedThread = (
                            from t in db.Threads
                            where t.Title.IndexOf(keyword) != -1
                            orderby t.Views descending
                            select t
                        ).Take(20).ToList<Thread>();

            return matchedThread; 
        }   

        //search tags 

        private List<Thread> getMatchingTags()
        {

            List<Thread> matchedThread = (from t in db.TagThreads
                                          where t.Tag.TagText.IndexOf(keyword) != -1
                                          orderby t.Thread.Views
                                          select t.Thread).Take(20).ToList<Thread>();

            return matchedThread;
        }
        
        //search content
        private List<Thread> getMatchingContent()
        {
            List<Thread> matchedThread = (
                            from t in db.Threads
                            where t.Content.IndexOf(keyword) != -1
                            orderby t.Views descending
                            select t
                        ).Take(20).ToList<Thread>();

            return matchedThread; 
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
