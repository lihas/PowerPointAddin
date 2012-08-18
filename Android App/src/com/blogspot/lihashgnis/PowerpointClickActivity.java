package com.blogspot.lihashgnis;

import java.io.IOException;
import java.net.DatagramPacket;
import java.net.DatagramSocket;
import java.net.InetSocketAddress;
import java.net.Socket;
import java.net.UnknownHostException;

import com.blogspot.lihashgnis.R;
import android.app.Activity;
import android.os.Bundle;
import android.widget.Button;
import android.widget.Toast;
import android.view.View;
import android.widget.EditText;
import java.io.*;

public class PowerpointClickActivity extends Activity {
    /** Called when the activity is first created. */
    String ip="";
    int port=0;
    DatagramSocket socktglob;
    BufferedWriter sockOutStreamGlob;
    InetSocketAddress addressglob;
    DatagramPacket requestglob,requestglob1,requestglob2,requestglob3;
    
	@Override
    public void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.main);
        Button btn = (Button) findViewById(R.id.button4);

        btn.setOnClickListener(new View.OnClickListener() {            
        	@Override
            public void onClick(View v) {
               EditText t1= (EditText) findViewById(R.id.editText1);
               String temp1=t1.getText().toString();
               String[] temp2=temp1.split(":",2);
               ip=temp2[0];
               port=Integer.parseInt(temp2[1]);
               Toast.makeText(getApplicationContext(), "ip= "+ip+" Port= "+port, Toast.LENGTH_LONG).show();
               
               //socket IO code:
             try{
              /* Socket sockt = new Socket(ip, Integer.parseInt(port));
             socktglob=sockt;              
             sockOutStreamGlob= new BufferedWriter(new OutputStreamWriter(socktglob.getOutputStream()));             
             sockOutStreamGlob.write('1');
             sockOutStreamGlob.flush();*/
            
            	
            	 InetSocketAddress address = new InetSocketAddress(ip,port);
                 
                 DatagramSocket socket = new DatagramSocket();
                 socktglob=socket;
                 
                String cmd="0"; 
                requestglob = new DatagramPacket(cmd.getBytes(), cmd.length(), address);
                socket.send(requestglob);
                
                cmd="1"; 
                requestglob1 = new DatagramPacket(cmd.getBytes(), cmd.length(), address);
                cmd="2"; 
                requestglob2 = new DatagramPacket(cmd.getBytes(), cmd.length(), address);
                cmd="3"; 
                requestglob3 = new DatagramPacket(cmd.getBytes(), cmd.length(), address);
                
                
                 				
            	 
             }catch (IOException e) {
            	 Toast.makeText(getApplicationContext(), "Exception-SaveIP Btn", Toast.LENGTH_LONG).show();
                 e.printStackTrace();
             } 
            }
        });
        //Listener for button 1 (mouse click)
        ((Button) findViewById(R.id.button1)).setOnClickListener(new View.OnClickListener() {
        		@Override
                public void onClick(View v) {
        			try {
        				//String cmd="1"; 
                        //requestglob = new DatagramPacket(cmd.getBytes(), cmd.length(), addressglob);
                        socktglob.send(requestglob1);
        			} catch (IOException e) {
        				Toast.makeText(getApplicationContext(), "Exception-mouse click Btn", Toast.LENGTH_LONG).show();
        		        e.printStackTrace();
        		    } 
        		
        		}
        		});
        
        //Listener for button 2 (prev slide)
        ((Button) findViewById(R.id.button2)).setOnClickListener(new View.OnClickListener() {
    		@Override
            public void onClick(View v) {
    			try {
    				//String cmd="2"; 
                    //DatagramPacket request = new DatagramPacket(cmd.getBytes(), cmd.length(), addressglob);
                    socktglob.send(requestglob2);
    			} catch (IOException e) {
    		    	Toast.makeText(getApplicationContext(), "Exception-prev slide Btn", Toast.LENGTH_LONG).show();
    		        e.printStackTrace();
    		    } 
    		
    		}
    		});
    
        //Listener for button 3 (next slide)
        ((Button) findViewById(R.id.button3)).setOnClickListener(new View.OnClickListener() {
    		@Override
            public void onClick(View v) {
    			try {
    			//String cmd="3"; 
                //DatagramPacket request = new DatagramPacket(cmd.getBytes(), cmd.length(), addressglob);
                socktglob.send(requestglob3);
    			}  catch (IOException e) {
    				Toast.makeText(getApplicationContext(), "Exception-prev slide Btn", Toast.LENGTH_LONG).show();    		        
    		        e.printStackTrace();
    		    } 
    		
    		}
    		}); 
        
	}
}