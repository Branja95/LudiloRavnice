import { Component, OnInit, NgZone } from '@angular/core';
import { NotificationService } from '../services/notification.service';  

@Component({
  selector: 'app-clock',
  templateUrl: './clock.component.html',
  styleUrls: ['./clock.component.css']
})
export class ClockComponent implements OnInit {

  ngOnInit() {
  }

  public currentMessage: string;  
  public allMessages: string;  
  public canSendMessage: Boolean;  
  // constructor of the class to inject the service in the constuctor and call events.  
  constructor(private notificationService: NotificationService, private _ngZone: NgZone) {  
      // this can subscribe for events  
      this.subscribeToEvents();  
      // this can check for conenction exist or not.  
      this.canSendMessage = notificationService.connectionExists;  
      // this method call every second to tick and respone tansfered to client.  
      //setInterval(() => {  
      //    this.notificationService.sendTime();  
      //}, 1000);  
  }  
  private subscribeToEvents(): void {  
      // if connection exists it can call of method.  
      this.notificationService.connectionEstablished.subscribe(() => {  
          this.canSendMessage = true;  
      });  
      // finally our service method to call when response received from server event and transfer response to some variable to be shwon on the browser.  
      this.notificationService.messageReceived.subscribe((message: string) => {  
          debugger;  
          this._ngZone.run(() => {  
              this.allMessages = message;  
          });  
      });  
  }  

  public sendStartToServer(): void {  
    this.notificationService.sendStart();  
  }  
}
