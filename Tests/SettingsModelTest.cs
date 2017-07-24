using MediaPlayer.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class SettingsModelTest
    {

        [TestMethod]
        public void ValidModelTest()
        {
            var settings = new Settings()
            {
                ScreenId = "Screen Id",
                CalledURL = "CalledURL",
                SecurityKey = "SecurityKey",
                ImagesDisplayTime = 25,
                CronUpdateTime = 15,
                DefaultClipURL = "www.test.com"
            };
            Assert.AreEqual(settings.AreNumericFieldsValid(), true);
            Assert.AreEqual(settings.AreNonNumericFieldsValid(), true);
        }

        [TestMethod]
        public void InValidModelTest()
        {
            var invalidSettings = new Settings()
            {
                ScreenId = "",
                CalledURL = "CalledURL",
                SecurityKey = "SecurityKey",
                ImagesDisplayTime = 25,
                CronUpdateTime = 15,
                DefaultClipURL = "www.test.com"
            };
            var invalidSettings2 = new Settings()
            {
                ScreenId = "ScreenID",
                CalledURL = "CalledURL",
                SecurityKey = "SecurityKey",
                ImagesDisplayTime = 25,
                CronUpdateTime = 0,
                DefaultClipURL = "www.test.com"
            };
            var invalidSettings3 = new Settings()
            {
                ScreenId = "ScreenID",
                CalledURL = "CalledURL",
                SecurityKey = "",
                ImagesDisplayTime = 0,
                CronUpdateTime = 15,
                DefaultClipURL = "www.test.com"
            };
            Assert.AreEqual(invalidSettings.AreNonNumericFieldsValid(), false);
            Assert.AreEqual(invalidSettings.AreNumericFieldsValid(), true);

            Assert.AreEqual(invalidSettings2.AreNonNumericFieldsValid(), true);
            Assert.AreEqual(invalidSettings2.AreNumericFieldsValid(), false);

            Assert.AreEqual(invalidSettings3.AreNonNumericFieldsValid(), false);
            Assert.AreEqual(invalidSettings3.AreNumericFieldsValid(), false);
        }



    }
}
