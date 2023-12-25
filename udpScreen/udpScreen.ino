/*
 * Se suscribe a un grupo UDP Multicast (239.1.2.3)
  Ver: https://www.iana.org/assignments/multicast-addresses/multicast-addresses.xhtml

      Al iniciar imprime por puerto serie el IP y puerto UDP que está escuchando
      Cada vez que recibe un mensaje por UDP imprime en el puerto serie:
      - El tipo de mensaje (Multicast)
      - El IP y puerto UDP del Sender
      - El IP y puerto UDP propio
      - El Payload (contenido) del mensaje

      Retransmite al grupo UDP lo que recibe en el puerto serie

      Si recibe un número entero o float lo guarda en la variable salto

      Cuando recibe input manda un al valor guardado en la var salto por el UDP.

  Ver proyecto completo en https://github.com/migrassi/ESP32-Unity-UDP
 */
#include <Arduino.h> // Esta linea no hace falta si se usa el entorno Arduino
#include "WiFi.h"
#include "AsyncUDP.h"
#include <Adafruit_GFX.h>
#include <Adafruit_SSD1306.h>


const char * ssid = "CoopWIFI-44a";//"S7YesicaAndroid";
const char * password = "ezup6664tv";//"yesica666";

const int puerto = 1234;

AsyncUDP udp;
String miBuffer;
float auxi;
String salto = "1";

#define LED_PIN         2
#define POTENTIOMETER_PIN  34  // ESP32 pin GIOP36 (ADC0) connected to Potentiometer pin
int sensorValue = 0;        // value read from the pot
int outputValue = 0;        // value output to the PWM (analog
//----------------------------------------Configure OLED screen size in pixels
#define SCREEN_WIDTH 128 //--> OLED display width, in pixels
#define SCREEN_HEIGHT 64 //--> OLED display height, in pixels
#define OLED_RESET     -1 // Reset pin # (or -1 if sharing Arduino reset pin)
Adafruit_SSD1306 display(SCREEN_WIDTH, SCREEN_HEIGHT, &Wire, OLED_RESET);
//----------------------------------------
void setup()
{
  pinMode(LED_PIN, OUTPUT);
  Serial.begin(115200);

  WiFi.mode(WIFI_STA);
  WiFi.begin(ssid, password);

  if (WiFi.waitForConnectResult() != WL_CONNECTED) {

  display.clearDisplay();
  display.setTextColor(WHITE);
  display.setTextSize(2);
  display.setCursor(37, 0);
  display.print("WiFi Failed");

  display.display();
  delay(2000);
  display.clearDisplay();
    
    Serial.println("WiFi Failed");
    while (1) {
      delay(1000);
    }
  }
  //---------------------------------------- SSD1306_SWITCHCAPVCC = generate display voltage from 3.3V internally.
  // Address 0x3C for 128x32 and Address 0x3D for 128x64.
  // But on my 128x64 module the 0x3D address doesn't work. What works is the 0x3C address.
  // So please try which address works on your module.
  if (!display.begin(SSD1306_SWITCHCAPVCC, 0x3C)) {
    Serial.println(F("SSD1306 allocation failed"));
    for (;;); //--> Don't proceed, loop forever
  }


  if (udp.listenMulticast(IPAddress(239, 1, 2, 3), puerto)) {
    Serial.print("Escuchando UDP en IP: ");
    Serial.println(WiFi.localIP());
    Serial.print("  Puerto:  ");
    Serial.println(puerto);

  //----------------------------------------
  display.clearDisplay();
  display.setTextColor(WHITE);
  display.setTextSize(2);
  display.setCursor(37, 0);
  display.print("ESP32");
  display.setTextSize(1);
  display.setCursor(13, 20);
  display.print("Listen UDP on IP: ");
  display.setCursor(7, 40);
  display.print("Port:");
  display.display();
  delay(5000);
  display.clearDisplay();

    udp.onPacket([](AsyncUDPPacket packet) {
      Serial.print("Tipo de paquete UDP: ");
      Serial.print(packet.isBroadcast() ? "Broadcast" : packet.isMulticast() ? "Multicast" : "Unicast");
      Serial.print(", De: ");
      Serial.print(packet.remoteIP());
      Serial.print(":");
      Serial.print(packet.remotePort());
      Serial.print(", To: ");
      Serial.print(packet.localIP());
      Serial.print(":");
      Serial.print(packet.localPort());
      Serial.print(", Longitud: ");
      Serial.print(packet.length());
      Serial.print(", Data: ");
      Serial.write(packet.data(), packet.length());
      Serial.println();
      //reply to the client
      packet.printf("Recibí %u bytes de datos", packet.length());
    });
    //Send multicast
    udp.print("Hola Grupo!");
  }


}

void loop()
{
  if (Serial.available()) {      // Si viene algo en el puerto Serial (USB),
    // udp.print(Serial.read());   // Manda por udp al grupo Multicast cada caracter que le llega
    //miBuffer = Serial.readString(); // Espera hasta el TimeOut. Es más lenta pero lee la cadena entera
    miBuffer = Serial.readStringUntil('\n');//Lee la cadena hasta el caracter indicado o toda por timeout si el caracter no está
  }

  if (miBuffer != "")//Si hay un string en miBuffer
  {
    udp.print(miBuffer);   // Lo manda por udp al grupo Multicast. Unity no recibe esto porque no se suscribe aparentemente
    udp.broadcastTo(miBuffer.c_str(), 1234); // Pero si recibe el broadcast
    auxi = miBuffer.toFloat(); // Además, si es convertible a un número, lo carga en una variable auxiliar

    if (auxi == 0) // Aunque evito que quede en cero
    {
      salto = "1";
    }
    else
    {
      salto = String(auxi);
    }

    miBuffer = "";


  }

  sensorValue = analogRead(POTENTIOMETER_PIN);
  outputValue = map(sensorValue, 1023, 0, 20, 0);

  if (outputValue >= 3 )
  {
    digitalWrite (LED_BUILTIN, HIGH);
    udp.broadcastTo(String(outputValue).c_str(), 1234);
    Serial.println(2);

    display.clearDisplay();
    display.setTextColor(WHITE);
    display.setTextSize(2);
    display.setCursor(14, 0);
    display.print("Mayor 3");
    display.display();
    delay(50);
  }
  else if (outputValue < 3 )
  {
    digitalWrite (LED_BUILTIN, LOW);
    udp.broadcastTo(String(outputValue).c_str(), 1234);
    Serial.println(1);
    display.clearDisplay();
    display.setTextColor(WHITE);
    display.setTextSize(2);
    display.setCursor(14, 0);
    display.print("Menor 3");
    display.display();
    delay(50);

  }

  //    Serial.print("outpuValue: ");
  //    Serial.print(outputValue);
  //    Serial.println();
}
