# tf_idf_csharp

El Proyecto tiene por objetivo Calcular el term frequency y el inverse document frecuency para un conjunto de documentos utilizando C#.

La motivación de este proyecto pasa por calcular manualmente el TF - IDF de un conjunto de documentos, pasando por la creación de un "vocabulario", el conteo de palabras dentro de un documento,  la determinación de frecuencia de termino (TF), el IDF de cada palabra,la normalización de estos resultados y la obtención de un TF-IDF normalizado.

    Existen librerias que entregan este calculo ya hecho, como por ejemplo SCIKIT-LEARN, que es una libreria para el uso Machine Learning con Python, en el siguiente repositorio: <a href="https://github.com/patovega/tf_idf_cosine_similarities" target="blank_">TF IDF Cosine similarities</> la utilizo para el calculo del TF IDF y la similutd de coseno para encontrar documentos similiares entre diferentes documentos.

# TF-IDF

    Tf-idf (del inglés Term frequency – Inverse document frequency), frecuencia de término – frecuencia inversa de documento (o sea, la frecuencia de ocurrencia del término en la colección de documentos), es una medida numérica que expresa cuán relevante es una palabra para un documento en una colección. <a href="https://es.wikipedia.org/wiki/Tf-idf" target="blank_">Wikipedia</>

# About this project

El siguiente proyecto corresponde a una aplicación de consola escrita en C#, bajo el alero de .NET 4.5.

Para la normalización de vectores en este proyecto se utiliza la distancia euclidiana o L2. Gracias a la normalización podemos expresar distancias entre documentos.
