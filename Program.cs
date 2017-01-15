using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;
using MySql.Data.MySqlClient;

namespace ITextSharpDraft
{
	public class Program
	{
		public static string ReadPdfFile(object fileName)
		{
			PdfReader reader = new PdfReader(fileName.ToString());
			string strText = null;

			for (int page = 1; page <= reader.NumberOfPages; page++) // Page has to be initialised as 1, not 0!!!
			{
				ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
				string s = PdfTextExtractor.GetTextFromPage(reader, page, its);

				s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));

				strText = strText + s;
			}
			reader.Close();
			return strText;
		}

		// ITextSharp methods and libraries used here****

		public static ArrayList ReadPdfFileArrayList(object fileName) // IDK why this can't be private
		{
			PdfReader reader = new PdfReader(fileName.ToString());
			//string strText = null;
			ArrayList texts = new ArrayList();

			for (int page = 1; page <= reader.NumberOfPages; page++) // Page has to be initialised as 1, not 0!!!
			{
				ITextExtractionStrategy its = new SimpleTextExtractionStrategy();
				string s = PdfTextExtractor.GetTextFromPage(reader, page, its);

				s = Encoding.UTF8.GetString(ASCIIEncoding.Convert(Encoding.Default, Encoding.UTF8, Encoding.Default.GetBytes(s)));

				texts.Add(s);

				// string s contains all the text from a single page in the pdf file, therefore
				// every increment within this for loop, will store all the text from a single page
				// into the texts arraylist. 
			}
			reader.Close();
			return texts;
		}

		//***************************

		public static int getNumberOfLines(char[] characters)
		{
			int numberOfLines = 0;

			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i] == '\n')
				{
					numberOfLines++;
				}
			}
			numberOfLines++; // Perhaps this can help take into account the very last line of text
							 // in a page?
			return numberOfLines;
		}

		public static string[] getLinesOfText(char[] characters, int numberOfLines)
		{
			int lineCounter = 0; // This should not exceed the numberOfLines
			StringBuilder text = new StringBuilder();
			string[] linesOfText = new string[numberOfLines];
			for (int i = 0; i < characters.Length; i++)
			{
				if (characters[i] == '\n')
				{
					// Once a new line character is reached, add all the characters currently in the
					// string builder into the string array
					linesOfText[lineCounter] = text.ToString();
					text.Clear(); // Then clear all the characters in the string builder at this 
								  // instant in order to gather all the characters from the next line

					// This checks to make sure the line counter doesn't exceed the number of lines
					// on a single page
					if (lineCounter < numberOfLines)
						lineCounter++;
					else
						break;
				}
				else
				{
					text.Append(characters[i]); // If no new line character is detected at
												// the current index, append the current character to the string builder
				}
			}

			// This makes sure to include the characters contained in the last line of text.
			linesOfText[lineCounter] = text.ToString();
			text.Clear();
			return linesOfText;
		}

		public static void Main(string[] args)
		{
			//Program test = new Program();

			try
			{
				string filepathzero = "PdfTestTwo.pdf"; // You don't have to specify file path if the file
														// itself is saved in the bin\debug folder for the project
				string filepath = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Programs\Draft C# Scanning\ITextSharpDraft\ITextSharpDraft\PdfTestTwo.pdf";
				string filepathtwo = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Sherwin Bayer CV.pdf";
				string filepaththree = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Watercare Bills\Watercare_Bill_Sample.pdf"; // "Account type" index 53
				string filepathfour = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Watercare Bills\Watercare_Bill_2047547-02_2017_Jan_05.pdf"; // 53
				string filepathfive = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Watercare Bills\Watercare_Bill_5107222-01_2017_Jan_06.pdf"; // 53
				string filepathsix = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Watercare Bills\Watercare_Bill_5163712-02_2017_Jan_09.pdf"; // not 53
				string filepathseven = @"C:\Users\Sherwin\Documents\Uni - Work Experience\Watercare Bills\Watercare_Bill_5126604-02_2017_Jan_04.pdf";
				//FileStream fs = File.Open(filepath, FileMode.Open);
				/*
                string pdfText = test.ReadPdfFile(filepath);
                Console.WriteLine("Here is the text from the pdf file: " + pdfText);

                if(pdfText.Contains("Hello, this"))
                {
                    int beginIndexHell = pdfText.IndexOf(",");
                    Console.WriteLine("Beginning index of Hell: " + beginIndexHell);
                }
                else if (pdfText.Contains("Hell"))
                {
                    Console.WriteLine("Testing");
                }
                

                int beginIndexHello = pdfText.IndexOf("Hello");
                Console.WriteLine("Beginning index of Hello: " + beginIndexHello);
                int noOfChars = "Hello".Length;

                int endIndexHello = beginIndexHello + noOfChars - 1;
                Console.WriteLine("Ending index of Hello: " + endIndexHello);

                int beginIndexI = pdfText.IndexOf("Hello"); // i
                Console.WriteLine("Beginning index of first 'Hello': " + beginIndexI);

                int beginIndexNextI = pdfText.IndexOf("Hello a", beginIndexI + 1); //Case Sensitive!
                //The starting index specified is inclusive when searching for the first occurence of
                //the value you are searching for.
                Console.WriteLine("Beginning index of second 'Hello': " + beginIndexNextI);
                */
				ArrayList pdftexts = ReadPdfFileArrayList(filepaththree);
				object[] textsArray = pdftexts.ToArray();

				// By converting the arraylist to an array, each index within the array
				// will essentially contain all the text from a single page from the pdf.

				string pageOne = textsArray[0].ToString();
				char[] pageOneChars = pageOne.ToCharArray();
				int length = pageOneChars.Length;
				string pageOneTrim = Regex.Replace(pageOne, "\\s*", "").Trim().Normalize(); // Must use regular
																							// expressions to trim white space characters in between a string, '\s*' is the
																							// notation for zero or more occurences of white space characters.
																							// I don't think normalize helps here...
				int newLineChars = getNumberOfLines(pageOneChars);

				// Since we only need to extract data which is all in the form of text from the pdf,
				// maybe we need to somehow find the starting and ending indices for the relevant
				// data that we need to extract and then use substrings?

				// Also 99% sure we will need to use the string's .Trim() method to get rid of all
				// the white spaces stored in that respective string.

				//int beginIndex = pageOne.IndexOf("S");
				//Console.WriteLine("Sherwin Bayer beginning index: " + beginIndex);

				StreamWriter sw = new StreamWriter(@"C:\Users\Sherwin\Documents\Uni - Work Experience\StreamWriter.txt");
				sw.WriteLine(pageOne);
				sw.Close();

				Console.WriteLine("Watercare Invoice, page 1: " + pageOne);
				Console.WriteLine("No. of characters on this page: " + length);
				Console.WriteLine("No. of lines on this page: " + newLineChars);

				string[] linesOfText = getLinesOfText(pageOneChars, newLineChars);
				int linesOfTextLength = linesOfText.Length;
				Console.WriteLine("The last line of text from the pdf is: " + linesOfText[linesOfTextLength - 1]);
				Console.WriteLine("Account type: " + linesOfText[53]);
				// It seems as though the last line in the page doesn't have a new line character,
				// hence linesOfText does not contain the text for the very last line, how to
				// include that last line of text? How to check when end of file has been reached
				// using a character array?

				//fs.Close();
				//string filepath = "C:\\Users\\Sherwin\\Documents\\Uni - Work Experience\\Programs\\Draft C# Scanning\\ITextSharpDraft\\ITextSharpDraft\\TestFile.txt";
				//StreamReader sr = new StreamReader(filepath);

				// Read the stream to a string, and write the string to the console.
				//string line = sr.ReadToEnd();
				//Console.WriteLine(line);
				//sr.Close();

			}
			catch (IOException e)
			{
				Console.WriteLine("Sorry, there was an error.");
				Console.WriteLine(e.Message);
			}

		}
	}
}
