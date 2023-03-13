using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;
    
    //-sources
    public AudioSource music_source;
    
    public AudioSource eff_source1;
    public AudioSource eff_source2;
    public AudioSource eff_source3;
    public AudioSource eff_source4;
    public AudioSource eff_source5;
    
    //-sounds
    public AudioClip mainscene;
    
    public AudioClip shooter_shoot;
    float shooter_shoot_last = float.MinValue;
    float shooter_shoot_dur = 0.5f;
    
    public AudioClip alien_melee;
    float alien_melee_last = float.MinValue;
    float alien_melee_dur = 0.5f;
    
    float min_time_diff = 0.1f; // NOTE(sftl): for the same clip, in seconds
    float max_sources = 2; // NOTE(sftl): max num of sources playing the same sound
    
    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
    }
    
    void Start()
    {
        music_source.clip = mainscene;
        music_source.loop = true;
        music_source.Play();
    }
    
    public void Play_GuardianShoot()
    {
        play_eff(shooter_shoot, ref shooter_shoot_last);
    }
    
    public void Play_AlienMelee()
    {
        play_eff(alien_melee, ref alien_melee_last);
    }
    
    void play_eff(AudioClip eff, ref float last_time)
    {
        if (Time.time - last_time < min_time_diff)      return;
        if (num_of_sources_playing(eff) >= max_sources) return;
        
        if (is_iddle(eff_source1))
        {
            play_eff(eff_source1, eff, ref last_time);
        }
        else if (is_iddle(eff_source2))
        {
            play_eff(eff_source2, eff, ref last_time);
        }
        else if (is_iddle(eff_source3))
        {
            play_eff(eff_source3, eff, ref last_time);
        }
        else if (is_iddle(eff_source4))
        {
            play_eff(eff_source4, eff, ref last_time);
        }
        else if (is_iddle(eff_source5))
        {
            play_eff(eff_source5, eff, ref last_time);
        }
    }
    
    void play_eff(AudioSource source, AudioClip eff, ref float last_time)
    {
        source.clip = eff;
        source.Play();
        last_time = Time.time;
    }
    
    // NOTE(sftl): for effect sources
    bool is_iddle(AudioSource source)
    {
        if (!source.isPlaying) return true; 
        
        // NOTE(sftl): isPlaying is not precise so we use a custom duration for each sound
        
        var clp = source.clip;
        if (clp == shooter_shoot)
        {
            if (Time.time > shooter_shoot_last + shooter_shoot_dur) return true;
        }
        else if (clp == alien_melee)
        {
            if (Time.time > alien_melee_last + alien_melee_dur) return true;
        }
        
        return false;
    }
    
    // NOTE(sftl): for effect sources
    int num_of_sources_playing(AudioClip clp)
    {
        int r = 0;
        
        if (eff_source1.clip == clp && !is_iddle(eff_source1)) r++;
        if (eff_source2.clip == clp && !is_iddle(eff_source2)) r++;
        if (eff_source3.clip == clp && !is_iddle(eff_source3)) r++;
        if (eff_source4.clip == clp && !is_iddle(eff_source4)) r++;
        if (eff_source5.clip == clp && !is_iddle(eff_source5)) r++;
        
        return r;
    }
}