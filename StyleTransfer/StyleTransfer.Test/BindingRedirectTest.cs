namespace PaintDotNet.Effects.ML.StyleTransfer.Test
{
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using System.Text.RegularExpressions;

    [TestClass]
    public class BindingRedirectTest
    {
        [TestMethod]
        public void ParsesXmlConfigIntoBindingRedirects()
        {
            var bindingRedirects = Plugin.AssemblyBindingRedirectHelper.GetBindingRedirects();
            Assert.IsNotNull(bindingRedirects);
            Assert.IsTrue(bindingRedirects.Count > 0);

            var token = new Regex(@"^[a-f0-9]{16}$", RegexOptions.Compiled);
            var version = new Regex(@"^(\d+\.){3}\d+$", RegexOptions.Compiled);

            bindingRedirects.ForEach(x =>
            {
                Assert.IsTrue(token.IsMatch(x.PublicKeyToken));
                Assert.IsTrue(version.IsMatch(x.RedirectToVersion));
                Assert.IsFalse(string.IsNullOrWhiteSpace(x.ShortName));
            });
        }
    }
}
