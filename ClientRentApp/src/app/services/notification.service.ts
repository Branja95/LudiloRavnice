import { Injectable, EventEmitter } from '@angular/core';
import * as signalR from "@aspnet/signalr";

import { environment } from '../../environments/environment';
import { env } from 'process'; 

declare var $: any;  

@Injectable({
    providedIn: 'root'
  })

export class NotificationService {
  
  private hubAccountConnection: signalR.HubConnection;
  private hubServiceConnection: signalR.HubConnection;
 
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
    this.hubAccountConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.endpointAccountHubConnection)
      .build();
    this.hubServiceConnection = new signalR.HubConnectionBuilder()
      .withUrl(environment.endpointRentVehicleHubConnection)
      .build();
  }

  public startConnection = () => {
    this.hubAccountConnection
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
    this.hubServiceConnection
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
      this.hubAccountConnection.on('newUserAccountToApprove', (data: number) => {  
          this.messageAccountReceived.emit(data);  
      });  
  }  

  private registerOnServerServiceEvents(): void {  
    this.hubServiceConnection.on('newRentAVehicleServiceToApprove', (data: number) => {  
        this.messageServiceReceived.emit(data);  
    });  
  }  
}
