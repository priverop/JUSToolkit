// Copyright (c) 2020 Priverop
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
// SOFTWARE.
using JUS.Tool.Utils;
using NUnit.Framework;

namespace JUS.Tests.Utils
{
    public class StringFunctionsTests
    {
        [Test]
        public void TestOriginalName()
        {
            const string nameCorrectPattern = "menu-option-victory00.png";
            Assert.That(StringFunctions.GetOriginalName(nameCorrectPattern), Is.EqualTo("victory00.png"));

            const string nameIncorrectPattern = "menu-optionvictory00.png";
            Assert.That(StringFunctions.GetOriginalName(nameIncorrectPattern), Is.Not.EqualTo("victory00.png"));

            const string nameIncorrectPattern2 = "menuoptionvictory00.png";
            Assert.That(StringFunctions.GetOriginalName(nameIncorrectPattern2), Is.Not.EqualTo("victory00.png"));
        }

        [Test]
        public void TestDemoName()
        {
            const string nameCorrectPattern = "demo-dn_02.png";
            Assert.That(StringFunctions.GetDemoName(nameCorrectPattern), Is.EqualTo("dn_02.png"));

            const string nameIncorrectPattern = "dn_02.png";
            Assert.That(StringFunctions.GetOriginalName(nameIncorrectPattern), Is.EqualTo("dn_02.png"));

            const string nameIncorrectPattern2 = "demo_dn_02.png";
            Assert.That(StringFunctions.GetOriginalName(nameIncorrectPattern2), Is.Not.EqualTo("dn_02.png"));
        }
    }
}
