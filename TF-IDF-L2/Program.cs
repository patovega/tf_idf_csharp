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
            List<List<string>> stemmedDocs;
       

            List<string> bagOFwords = CreateVocabulary(out stemmedDocs);
            Dictionary<string, double> tf = CalculateTermFrequency(bagOFwords, stemmedDocs);
            double[][] tf_idf_vectors = TransformToTFIDFVectors(stemmedDocs, tf);
            double[][] inputs = Normalize(tf_idf_vectors);


            string[] documents = ScantTxtFiles();

            for (int index = 0; index < inputs.Length; index++)
            {
                Console.WriteLine(documents[index]);

                int wordIndex = 0;
                
                foreach (double value in inputs[index])
                {
                  
                   

                    if (wordIndex < stemmedDocs[index].Count() )
                    {
                        var wordDetail = stemmedDocs[index][wordIndex] + " = " + value;
                        Console.WriteLine(wordDetail);
                       
                    }
                    wordIndex = wordIndex + 1;

                }

                Console.WriteLine("\n");
            }

            Console.ReadKey();
        }

        private static Dictionary<string,double> CalculateTermFrequency(List<string> BagOFwords, List<List<string>> stemmedDocs)
        {
            Console.WriteLine("Calculando TF " + DateTime.Now);
            if (vocabularyIDF.Count == 0)
            {
                var docIndex = 0;
  
                foreach (var term in BagOFwords)
                {
                    double numberOfDocsContainingTerm = stemmedDocs.Where(d => d.Contains(term)).Count();
                    vocabularyIDF[term] = Math.Log((double)stemmedDocs.Count / ((double)numberOfDocsContainingTerm));

                    docIndex = docIndex + 1;
                }
            }
            return vocabularyIDF;
        }


        private static double[][] TransformToTFIDFVectors(List<List<string>> stemmedDocs, Dictionary<string, double> vocabularyIDF)
        {
            Console.WriteLine("Create TF vectors " + DateTime.Now);
            //ada documento en un vector
            List<List<double>> vectors = new List<List<double>>();
            foreach (var doc in stemmedDocs)
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
            Console.WriteLine("Retornando VECTORES IDF " + DateTime.Now);
            return vectors.Select(v => v.ToArray()).ToArray();
        }

        
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

        //L2 NORM
        public static double[] ApplyL2Norm(double[] vector)
        {
            List<double> result = new List<double>();

            double sumSquared = 0;
            foreach (var value in vector)
            {
                sumSquared += value * value;
            }

            double SqrtSumSquared = Math.Sqrt(sumSquared);

            foreach (var value in vector)
            {
                // Aplicamos norma L2
                // L2-norm: Xi = Xi / Sqrt(X0^2 + X1^2 + .. + Xn^2)
                result.Add(value / SqrtSumSquared);
            }

            return result.ToArray();
        }


        public static List<string> CreateVocabulary(out List<List<string>> stemmedDocs)
        {
            List<string> vocabulary = new List<string>();
            Dictionary<string, int> wordCountList = new Dictionary<string, int>();
            stemmedDocs = new List<List<string>>();
            
            string[] files = ScantTxtFiles();
            var path = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.FullName;
            path = path + "/dataset/";

            int docIndex = 0;

            foreach (var doc in files)
            {
                List<string> stemmedDoc = new List<string>();

                docIndex++;

                var content = File.ReadAllText(path + doc);
                var wordPattern = new Regex(@"\w+");

                List<string> words = new List<string>();

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
                            wordCountList.Add(word, 0);
                        }

                        stemmedDoc.Add(word);
                    }
                }

                stemmedDocs.Add(stemmedDoc);
            }

            foreach (var item in wordCountList)
            {
                vocabulary.Add(item.Key);
            }


            Console.WriteLine("Retornando Vocabulary " + DateTime.Now);
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

    }
}
