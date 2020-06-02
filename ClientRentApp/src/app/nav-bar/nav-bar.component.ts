import { Component, OnInit, NgZone } from '@angular/core';
import { Router } from '@angular/router';
import { NotificationService } from '../services/notification.service';  
import { NavBarService } from '../services/nav-bar.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
  providers: [NavBarService, NotificationService]
})
export class NavBarComponent implements OnInit {

  private username: string;
  private role : string;

  private showNotificationsAccount: boolean;
  private showNotificationsService: boolean;

  public serviceToApprove: number;  
  public accountToApprove: number;  
  public canSendMessage: Boolean;  

  constructor(private navBarService: NavBarService, private router: Router, private notificationService: NotificationService, private _ngZone: NgZone) { 
    
  }

  ngOnInit() {

    this.username = null;
    this.role = null;
    this.serviceToApprove = 0;
    this.showNotificationsService = false;
    this.accountToApprove = 0;
    this.showNotificationsAccount = false;

  }

  private subscribeToAccountEvents(): void {  
    // if connection exists it can call of method.  
    this.notificationService.connectionEstablished.subscribe(() => {  
        this.canSendMessage = true;  
    });  
    // finally our service method to call when response received from server event and transfer response to some variable to be shown on the browser.  
    this.notificationService.messageAccountReceived.subscribe((message: number) => {  
        this._ngZone.run(() => {  
          this.accountToApprove = message;
          this.showNotificationsAccount = true;  
        });  
    });  
  }  

  private subscribeToServiceEvents(): void {  
    // if connection exists it can call of method.  
    this.notificationService.connectionEstablished.subscribe(() => {  
        this.canSendMessage = true;  
    });  
    // finally our service method to call when response received from server event and transfer response to some variable to be shown on the browser.  
    this.notificationService.messageServiceReceived.subscribe((message: number) => {  
        this._ngZone.run(() => {  
          this.serviceToApprove = message;
          this.showNotificationsService = true;  
        });  
    });  
  }  

  resetNotificationsService() : void {
    this.showNotificationsService = false;
  }

  visibleNotificationsService()
  {
    return this.showNotificationsService;
  }

  resetNotificationsAccount() : void {
    this.showNotificationsAccount = false;
  }

  visibleNotificationsAccount()
  {
    return this.showNotificationsAccount;
  }

  isLogged() : boolean
  {
    if(!localStorage.jwt)
    {
      return false;
    }
    else
    {
      if(!this.username)
      {
        this.username = localStorage.username;
        //this.startHubConnection();      
        this.getNumOfAccAndServiceForApproval();       
      }
      return true;
    }
  }

  private startHubConnection() : void {
    if(localStorage.role == 'Administrator')
    {
      this.notificationService.connection.qs = { "token" : `Bearer ${localStorage.jwt}` };
      this.notificationService.startConnection()
      this.subscribeToAccountEvents();  
      this.subscribeToServiceEvents();
      this.canSendMessage = this.notificationService.connectionExists;  
    }
  }

  getNumOfAccAndServiceForApproval(){
    if(localStorage.role == 'Administrator'){
      this.navBarService.getMethodAccountForApproval()
      .subscribe(
        res => {
          this.accountToApprove = res as number;
          if(this.accountToApprove > 0){
            this.showNotificationsAccount = true;
          }
        },
        error => {
          console.log(error.error.Message);
        }
      );
      
      this.navBarService.getMethodServiceForApproval()
      .subscribe(
        res => {
          this.serviceToApprove = res as number;
          if(this.serviceToApprove > 0){
            this.showNotificationsService = true;
          }
        },
        error => {
          console.log(error);
        }
      );
    }
  }

  isAdmin() : boolean{
    if(localStorage.role == "Administrator")
    {
      return true;
    }

    return false ;
  }

  logoutUser(){
    this.navBarService.postMethodLogout().subscribe(
      res => {
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      })
    localStorage.clear();
    this.username = null;
    this.router.navigate(['/RentVehicle']);
  }

}