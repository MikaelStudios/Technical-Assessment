using UnityEngine;
using UnityEngine.Serialization;

[System.Serializable]
public class MovieData
{
    public int id { get; set; }
    public string title { get; set; }

    public bool adult { get; set; }
    public object backdrop_path { get; set; }
    public object belongs_to_collection { get; set; }
    public int budget { get; set; }
    public Genres[] genres { get; set; }
    public string homepage { get; set; }
    public string imdb_id { get; set; }
    public string[] origin_country { get; set; }
    public string original_language { get; set; }
    public string original_title { get; set; }
    public string overview { get; set; }
    public double popularity { get; set; }
    public string poster_path { get; set; }
    public Production_companies[] production_companies { get; set; }
    public Production_countries[] production_countries { get; set; }
    public string release_date { get; set; }
    public int revenue { get; set; }
    public int runtime { get; set; }
    public Spoken_languages[] spoken_languages { get; set; }
    public string status { get; set; }
    public string tagline { get; set; }
    public bool video { get; set; }
    public double vote_average { get; set; }
    public int vote_count { get; set; }
    
    public Sprite posterSprite { get; set; }
}


public class Genres
{
    public int id { get; set; }
    public string name { get; set; }
}

public class Production_companies
{
    public int id { get; set; }
    public object logo_path { get; set; }
    public string name { get; set; }
    public string origin_country { get; set; }
}

public class Production_countries
{
    public string iso_3166_1 { get; set; }
    public string name { get; set; }
}

public class Spoken_languages
{
    public string english_name { get; set; }
    public string iso_639_1 { get; set; }
    public string name { get; set; }
}
