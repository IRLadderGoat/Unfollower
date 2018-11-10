using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using System;
using System.Threading;

namespace Instagram_Unfollower
{
    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 1)
            {
                Console.WriteLine("Pass in the following variables 'Unfollower.exe username password'");
                Console.ReadKey();
                return;
            }
            string Base_URL = "https://www.instagram.com/";

            IWebDriver driver = new ChromeDriver
            {
                Url = "https://www.instagram.com/accounts/login/"
            };

            Thread.Sleep(1000);

            Console.WriteLine("Starting login as {0}", args[0]);

            driver.FindElement(By.Name("username")).SendKeys(args[0]);
            driver.FindElement(By.Name("password")).SendKeys(args[1]);

            driver.FindElement(By.XPath("//button[@type='submit']")).Submit();

            Thread.Sleep(2000);

            if (driver.Url == "https://www.instagram.com/accounts/onetap/?next=%2F")
            {
                driver.FindElement(By.XPath("//button[contains(text(), 'Ikke nu')]")).Click();
            }
            else if(driver.Url != "https://www.instagram.com/")
            {
                Console.WriteLine("Could not log in try restarting the program");
                Console.ReadKey();
                return;
            }
            Console.WriteLine("Succesfully logged in as {0}", args[0]);

            var brr = string.Format("{0}{1}", Base_URL.ToLower(), args[0].ToLower());
            driver.Navigate().GoToUrl(brr);

            string FollowLink = string.Format("//a[@href='/{0}/following/']", args[0].ToLower());

            while (driver.Url != brr + "/following/")
            {
                Console.WriteLine("Attempting at viewing followed");
                driver.FindElement(By.XPath(FollowLink)).Click();
                Thread.Sleep(1000);
            }

            Random rnd = new Random();
            int unfollowed = 0;
            Thread.Sleep(10000);
            int failedAttempts = 0;

            while (true)
            {
                try
                {
                    var Followed = driver.FindElement(By.XPath("//button[contains(text(), 'Følger')]"));
                    if (Followed == null)
                    {
                        break;
                    }
                    Actions action = new Actions(driver);
                    action.MoveToElement(Followed);
                    action.Perform();

                    Followed.Click();

                    if (driver.FindElement(By.XPath("//button[contains(text(), 'Følg ikke længere')]")) != null)
                    {
                        driver.FindElement(By.XPath("//button[contains(text(), 'Følg ikke længere')]")).Click();
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Looking for more followed users...");
                    if(failedAttempts > 5)
                    {
                        break;
                    }
                    else
                    {
                        failedAttempts++;
                        continue;
                    }
                }
                
                unfollowed++;
                var wait = rnd.Next(15, 30) * 1000;
                Console.WriteLine("Unfollowed: {0} \n Waiting {1} seconds till next unfollow", unfollowed, wait / 1000);
                Thread.Sleep(wait);
            }
            Console.WriteLine("Unfollowed {0} in total", unfollowed);
            Console.ReadKey();

        }
    }
}
