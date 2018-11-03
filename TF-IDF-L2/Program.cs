using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TF_IDF_L2
{
    public class Program
    {
    
        private static Dictionary<string, double> vocabularyIDF = new Dictionary<string, double>();

        static void Main(string[] args)
        {
            List<List<string>> docsDetails;
            string[] documents;

            List<string> bagOFwords = CreateVocabulary(out docsDetails, out documents);
            Dictionary<string, double> tf = CalculateTermFrequency(bagOFwords, docsDetails);
            double[][] tf_idf_vectors = TransformToTFIDFVectors(docsDetails, tf);
            double[][] tf_idf_normalized = Normalize(tf_idf_vectors);


            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n \n TF IDF with L2 Norm \n");
            Console.ResetColor();

            for (int index = 0; index < tf_idf_normalized.Length; index++)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(documents[index]);
                Console.ResetColor();

                int wordIndex = 0;
                
                foreach (double value in tf_idf_normalized[index])
                {
                    if (wordIndex < docsDetails[index].Count() )
                    {
                        PrintValues(docsDetails[index][wordIndex], value);
                    }

                    wordIndex = wordIndex + 1;
                }

                Console.WriteLine("\n");
            }

            Console.ReadKey();
        }

        /// <summary>
        /// Calculate TF IDF for each words inside the documents.
        /// </summary>
        /// <param name="BagOFwords">List<string> with all the words in all files</param>
        /// <param name="documents">List<List<string>> with each file and his words.</param>
        /// <returns></returns>
        private static Dictionary<string,double> CalculateTermFrequency(List<string> BagOFwords, List<List<string>> documents)
        {
         
            if (vocabularyIDF.Count == 0)
            {
                var docIndex = 0;
  
                foreach (var term in BagOFwords)
                {
                    double numberOfDocsContainingTerm = documents.Where(d => d.Contains(term)).Count();
                    vocabularyIDF[term] = Math.Log((double)documents.Count / ((double)numberOfDocsContainingTerm));

                    docIndex = docIndex + 1;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n TF IDF NON NORMALIZE \n");
            Console.ResetColor();

            foreach (var v in vocabularyIDF)
            {
                PrintValues(v.Key, v.Value);
            }

            return vocabularyIDF;
        }

        /// <summary>
        /// Transform words in vectors values
        /// </summary>
        /// <param name="documents">Documents with the content of them separete by words.</param>
        /// <param name="vocabularyIDF">Dictionary with key = word and value = idf</param>
        /// <returns>Double[][] represents the documents in vectors.</returns>
        private static double[][] TransformToTFIDFVectors(List<List<string>> documents, Dictionary<string, double> vocabularyIDF)
        {

            List<List<double>> vectors = new List<List<double>>();

            foreach (var doc in documents)
            {
                List<double> vector = new List<double>();

                foreach (var vocab in vocabularyIDF)
                {
                    double tf = doc.Where(d => d == vocab.Key).Count();
                    double tfidf = tf * vocab.Value;

                    vector.Add(tfidf);
                }

                vectors.Add(vector);
            }

            return vectors.Select(v => v.ToArray()).ToArray();
        }

        
        /// <summary>
        /// Normalize the vectors with L2 norm.
        /// </summary>
        /// <param name="vectors">Represents a document how a vector.</param>
        /// <returns>normalized vector.</returns>
        public static double[][] Normalize(double[][] vectors)
        {

            List<double[]> normalizedVectors = new List<double[]>();
            foreach (var vector in vectors)
            {
                var normalized = ApplyL2Norm(vector);
                normalizedVectors.Add(normalized);
            }
        
            return normalizedVectors.ToArray();
        }

        /// <summary>
        /// Apply L2 norm to vectors, norms are useful because they are used to express distances
        /// </summary>
        /// <param name="vector">Double[] vector</param>
        /// <returns>Vector normalized</returns>
        public static double[] ApplyL2Norm(double[] vector)
        {
            List<double> result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared +=  Math.Pow(value,2);
            }

            double SqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {
                // L2-norm: Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
                result.Add(value / SqrtSumSquared);
            }

            return result.ToArray();
        }

        /// <summary>
        /// Take documentes from dataset/ folder and analize each word of them.
        /// </summary>
        /// <param name="documents">Out, name of files and his content.</param>
        /// <returns>List<string> vocabulary, includes words and frequency of each words from files. </returns>
        public static List<string> CreateVocabulary(out List<List<string>> documents, out string[] files)
        {
            List<string> vocabulary = new List<string>();
            Dictionary<string, int> wordCountList = new Dictionary<string, int>();
            documents = new List<List<string>>();
            files = new string[0];
            files = ScantTxtFiles();
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            path = path + "/dataset/";


            int docIndex = 1;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n Numbers of DOCS: " + files.Count() + " \n");
            Console.ResetColor();

            foreach (var doc in files)
            {
                List<string> document = new List<string>();

                var content = File.ReadAllText(path + doc);
                var wordPattern = new Regex(@"\w+");

                List<string> words = new List<string>();

                //use regex MATCH for take each word inside the document.
                foreach (Match match in wordPattern.Matches(content.ToLower()))
                {
                    // If word is a stop word its ignored for the TF IDF 
                    if (StopWords.Words.Contains(match.Value.ToLower()))
                    {
                        continue;
                    }

                    string word = match.Value.ToLower();
                    words.Add(word);

                    if (word.Length > 0)
                    {
                        // Build the word count list.
                        if (wordCountList.ContainsKey(word))
                        {
                            wordCountList[word]++;
                        }
                        else
                        {
                            wordCountList.Add(word, 1);
                        }

                        document.Add(word);
                    }
                }

                documents.Add(document);
                docIndex++;
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("\n TF Term Frequency \n");
            Console.ResetColor();

            foreach (var item in wordCountList)
            {
                vocabulary.Add(item.Key);
                PrintValues(item.Key, item.Value);
            }

            return vocabulary;
        }

        /// <summary>
        ///  Scan the files with txt extension inside the dataset folder.
        /// </summary>
        /// <returns>string[], contains the name of .txt files</returns>
        private static string[] ScantTxtFiles()
        {
            var docs = new string[0];

            try
            {
                var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
                path = path + "/dataset";
                docs = Directory.GetFiles(path, "*.txt")
                                          .Select(Path.GetFileName)
                                          .ToArray();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return docs;
        }

        /// <summary>
        /// Print <key-values> of a dicctionary using console.colors
        /// </summary>
        /// <param name="key">string</param>
        /// <param name="value">double</param>
        private static void PrintValues(string key, double value)
        {
            Console.ResetColor();
            Console.Write(" Word: ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(key);

            Console.ResetColor();

            Console.Write(" Value: ");

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write(value);

            Console.WriteLine("");
            Console.ResetColor();
        }

    }
}
