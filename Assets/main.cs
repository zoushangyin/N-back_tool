using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Threading;
using Random=System.Random;

public class main : MonoBehaviour
{
    // Start is called before the first frame update

    
    public GameObject Panel_nback;
    
    public InputField nback_time;
    //public Text text_savepath;
    public Image button_back;
    

    int nback_current_ans;
    int nback_correct;
    int back_n;
    List<int> nback_input;
    int nback_feedback;
    bool feedback_switch;
    int nback_sec;
    bool event_flag;
    int[] ans_list;
    Color grey = new Color((221/255f),(218/255f),(218/255f),(255/255f));
    Color red = new Color((248/255f),(186/255f),(186/255f),(255/255f));

    //StreamWriter data = new StreamWriter("test.csv", true);
    //string savepath;
    

    void Start()
    {
        
        activate_nback();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Panel_nback.activeSelf && feedback_switch){
            if(nback_correct != nback_current_ans && nback_feedback == 1){
                button_back.color = red;
            }
        }
    }

    // UI manage
    private void activate_nback(){
        
        Panel_nback.SetActive(true);
        nback_set_default();
    

    }

    private List<int> generate_position_list(int back_n, int time_sec)
	{
	    List<int> pos_list = new List<int>();
	    Random ran = new Random();
	    for (int i=0; i<back_n; i++){
	        pos_list.Add(ran.Next(9));
	    }
	    Random ran_ = new Random();
	    for (int i=0; i<(time_sec/2-back_n); i++){
	        int n = select_position(pos_list[i], ran_);
	        pos_list.Add(n);
	    }
	    
	    return pos_list;
	}
	
	private int select_position(int previous_number, Random ran)
	{
	    
	    var list = generate_array(previous_number);
	    // random index
	    lock(ran);
	    int i = ran.Next(12);
	    return list[i];
	    
	}

    private int[] generate_answer(int[] pos_list, int back_n)
	{
	    List<int> answer_list = new List<int>();
	    for(int i=0; i<pos_list.Length; i++){
	        if(i<back_n){
	            answer_list.Add(0);
	        }
	        else{
	            if(pos_list[i] == pos_list[i-back_n]){
	                answer_list.Add(1);
	            }
	            else{
	                answer_list.Add(0);
	            }
	        }
	    }
	    return answer_list.ToArray();
	    
	}
	
	private List<int> generate_array(int t)
	{
		List<int> list = new List<int>();
		for(int i=0; i<9; i++)
		{
			if(i==t)
			{
				for (int j=0; j<4; j++)
				{
					list.Add(i);
				}
			}
			else{
				list.Add(i);
			}
		}
		return list;
	}

    
    private void nback_set_default()
    {
        nback_clear();
        event_flag = false;
        back_n = 2;
        
        feedback_switch = true; // default
        nback_time = nback_time.GetComponent<InputField> ();
        nback_time.text = "300";
        nback_sec = 300;

        button_back = button_back.GetComponent<Image> ();
        button_back.color = grey;
        nback_input = new List<int>();

    }
    
    private void nback_clear()
    {
        nback_feedback = 0;
        for (int i = 0; i < 9; i++)
        {
            // GameObject originalGameObject = GameObject.Find("Panel_nback");
            GameObject img = Panel_nback.transform.Find(String.Format("Image_{0}", i)).gameObject;
            if(img != null){
                img.SetActive(false);
            }
            else{
                Debug.Log("null");
            }
            
        }
    }
    

    private IEnumerator nback_show_multiple(int[] list, int[] ans_correct)
    {
        for(int i = 0; i<list.Length; i++)
        {
            //Debug.Log(get_ts());
            var t1 = get_ts();
            button_back.color = grey;
            nback_current_ans = 0;
            nback_correct = ans_correct[i];
            GameObject img = Panel_nback.transform.Find(String.Format("Image_{0}", list[i])).gameObject;
            if(img != null){
                img.SetActive(true);
            }
            else{
                Debug.Log("null");
            }
            //img.SetActive(true);
            yield return new WaitForSeconds(1);
            img.SetActive(false);
            nback_feedback = 1;
            var delta_t = (float)(get_ts() - t1); 
            
            //Debug.Log(nback_current_ans);
            nback_input.Add(nback_current_ans);
            
            
            yield return new WaitForSeconds(2-delta_t);
            nback_feedback = 0;
            if (!event_flag){
                break;
            }
            //Debug.Log(get_ts() - t1);
            
        }
        // save ans_list and nback_input to file
        
    }

    public void Dropdown_back_n(int val)
    {
        if (val == 0){
            back_n = 2;
            
        }
        if (val == 1){
            back_n = 3;
            
        }
    }

    public void Dropdown_feedback(int val)
    {
        if (val == 0){
            feedback_switch = true;
            
        }
        if (val == 1){
            feedback_switch = false;
            
        }
    } 

    public void InputTime()
    {
        nback_sec = int.Parse(nback_time.text);
        
    }

    public void ButtonStart()
    {
        event_flag = true;
        //writetime("-1");
        var pos_list = generate_position_list(back_n, nback_sec);
        int[] pos_array = pos_list.ToArray();
        ans_list = generate_answer(pos_array, back_n);

        Debug.Log(feedback_switch);
        Debug.Log(back_n);
        Debug.Log(nback_sec);

        StartCoroutine(nback_show_multiple(pos_array, ans_list));
        
        
    }

    public void ButtonEnd()
    {
        //writetime("-1");
        event_flag = false;
        nback_clear();
        // save ans_list and nback_input to file
        
        
    }

    public void ButtonNback()
    {
        nback_current_ans = 1;
        //Debug.Log(nback_current_ans);
    }

    

    /*
    private void writetime(string rating)
    {
        if(event_flag){
            
            //Debug.Log(rating);
            var ts_sting = get_ts();
            text_ts.text = ts_string;
            //Debug.Log(ts_string);
            
            using (StreamWriter stream = new StreamWriter(savepath, true))
            {
                stream.WriteLine(String.Format("{0},{1},{2},{3}", sub_no.text, ex_no.text, ts_string, rating));
            }
            

        }
    }
    */

    private double get_ts()
    {
        double ts = (double)DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000;
        //string ts_string = String.Format("{0:0.000}", ts);
        return ts;
    }

    
}
