import { Injectable, EventEmitter } from '@angular/core';

declare var $: any;  

@Injectable({
    providedIn: 'root'
  })

export class NotificationService {

  private proxy: any;  
  private proxyName: string = 'Notifications';  
  public connection: any;  
  // create the Event Emitter  
  public messageAccountReceived: EventEmitter < number > ;  
  public messageServiceReceived: EventEmitter < number > ;  
  public connectionEstablished: EventEmitter < Boolean > ;  
  public connectionExists: Boolean;  

  constructor() {  
    // Constructor initialization  
    this.connectionEstablished = new EventEmitter < Boolean > ();  
    this.messageAccountReceived = new EventEmitter < number > ();  
    this.messageServiceReceived = new EventEmitter < number > ();  
    this.connectionExists = false;  
    // create hub connection  
    //this.connection = $.hubConnection("https://localhost:5001/");  
    //this.connection.qs = { "token" : "Bearer eyJ0eXAiOiJKV1QiLCJhbGciOiJodHRwOi8vd3d3LnczLm9yZy8yMDAxLzA0L3htbGRzaWctbW9yZSNobWFjLXNoYTUxMiJ9.eyJuYW1laWQiOiJhZG1pbiIsInVuaXF1ZV9uYW1lIjoiYWRtaW4iLCJodHRwOi8vc2NoZW1hcy5taWNyb3NvZnQuY29tL2FjY2Vzc2NvbnRyb2xzZXJ2aWNlLzIwMTAvMDcvY2xhaW1zL2lkZW50aXR5cHJvdmlkZXIiOiJBU1AuTkVUIElkZW50aXR5IiwiQXNwTmV0LklkZW50aXR5LlNlY3VyaXR5U3RhbXAiOiI5NjZjM2U5MC0yMWVmLTQ2OWEtOGFlMC0zMDVmYmE5ZWJlZjYiLCJyb2xlIjoiQWRtaW4iLCJpc3MiOiJodHRwOi8vbG9jYWxob3N0OjUxNjgwIiwiYXVkIjoiYldsc1lYTm9hVzQ9IiwiZXhwIjoxNTI4ODAyNzg1LCJuYmYiOjE1Mjg3MTYzODV9.R4YkZjxyw550quPqakw-RZ-M0Lc4R8oviyTNZGFqFzj35eg_i1HAv07urPivqfsybq40HEnBPasL8nNSqLak9A" };
    // create new proxy as name already given in top  
    //this.proxy = this.connection.createHubProxy(this.proxyName);  
    // register on server events  
    //this.registerOnServerAccountEvents();  
    //.registerOnServerServiceEvents();
    // call the connecion start method to start the connection to send and receive events.  
    //this.startConnection();  
  }  

  sendStart() : void {
    this.proxy.invoke("TimeServerUpdates");
  }

  // check in the browser console for either signalr connected or not  
  public startConnection(): void {  
      this.connection.start().done((data: any) => {  
          console.log('Now connected ' + data.transport.name + ', connection ID= ' + data.id);  
          this.connectionEstablished.emit(true);  
          this.connectionExists = true;  
      }).fail((error: any) => {  
          console.log('Could not connect ' + error);  
          this.connectionEstablished.emit(false);  
      });  
  }  
  private registerOnServerAccountEvents(): void {  
      this.proxy.on('newUserAccountToApprove', (data: number) => {  
          console.log('Received in NotificationService: ' + JSON.stringify(data));  
          this.messageAccountReceived.emit(data);  
      });  
  }  

  private registerOnServerServiceEvents(): void {  
    this.proxy.on('newRentVehicleServiceToApprove', (data: number) => {  
        console.log('Received in NotificationService: ' + JSON.stringify(data));  
        this.messageServiceReceived.emit(data);  
    });  
    }  
}
