import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from "@aspnet/signalr";

declare var $: any;  

@Injectable({
    providedIn: 'root'
  })

export class NotificationService {
  
  private hubConnection: signalR.HubConnection;

  private proxy: any;  
  private proxyName: string = 'notificationHub';  
  public connection: any;  
  // create the Event Emitter  
  public messageAccountReceived: EventEmitter < number > ;  
  public messageServiceReceived: EventEmitter < number > ;  
  public connectionEstablished: EventEmitter < Boolean > ;  
  public connectionExists: Boolean;  

  constructor() {  
    this.connectionExists = false;  
    this.connectionEstablished = new EventEmitter < Boolean > ();  
    this.messageAccountReceived = new EventEmitter < number > ();  
    this.messageServiceReceived = new EventEmitter < number > ();  

    this.buildConnection();
    this.registerOnServerAccountEvents(); 
    this.registerOnServerServiceEvents(); 
  }  

  public buildConnection = () => {
    this.hubConnection = new signalR.HubConnectionBuilder()
      .withUrl("https://localhost:44366/notificationHub")
      .build();
  }

  public startConnection = () => {
    this.hubConnection
      .start()
      .then(() => {
        this.connectionEstablished.emit(true);  
        this.connectionExists = true;  
      })
      .catch(err => {
        setTimeout(function(){
          this.startConnection();
        }, 500);
      })
  }

  private registerOnServerAccountEvents(): void {  
      this.hubConnection.on('newUserAccountToApprove', (data: number) => {  
          this.messageAccountReceived.emit(data);  
      });  
  }  

  private registerOnServerServiceEvents(): void {  
    this.hubConnection.on('newRentAVehicleServiceToApprove', (data: number) => {  
        this.messageServiceReceived.emit(data);  
    });  
  }  
}
