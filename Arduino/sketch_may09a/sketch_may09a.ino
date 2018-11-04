void setup()
{
 Serial.begin(9600);
  pinMode(13,OUTPUT);
 
}
 

 
void loop()
{
  if(Serial.available())
 {
    switch(Serial.read())
    {
      case '1':
        digitalWrite(13,HIGH);     
        break;
       
      case '2':
        digitalWrite(13,LOW);     
        break;
       
    }
 }
}
