import { Component, OnInit, NgZone } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';

import { NotificationService } from '../services/notification.service';  

import { NavBarService } from '../services/nav-bar.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
  providers: [NavBarService, NotificationService]
})
export class NavBarComponent implements OnInit {

  private username: string
  private notificationCount: number

  private showNotifications: boolean

  public accountToApprove: number;  
  public canSendMessage: Boolean;  

  constructor(private navBarService: NavBarService, private router: Router, private notificationService: NotificationService, private _ngZone: NgZone) { 
    
  }

  ngOnInit() {
    this.accountToApprove = 0;
    this.username = null;
    this.showNotifications = false;
  }

  private subscribeToEvents(): void {  
    // if connection exists it can call of method.  
    this.notificationService.connectionEstablished.subscribe(() => {  
        this.canSendMessage = true;  
    });  
    // finally our service method to call when response received from server event and transfer response to some variable to be shown on the browser.  
    this.notificationService.messageReceived.subscribe((message: number) => {  
        this._ngZone.run(() => {  
          this.accountToApprove = message;
          this.showNotifications = true;  
        });  
    });  
  }  

  resetNotifications() : void {
    this.showNotifications = false;
  }

  visibleNotifications()
  {
    return this.showNotifications;
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
        this.startHubConnection();
      }
      return true;
    }
  }

  private startHubConnection() : void {
    if(localStorage.role == 'Admin')
    {
      this.notificationService.connection.qs = { "token" : `Bearer ${localStorage.jwt}` };
      this.notificationService.startConnection()
      this.subscribeToEvents();  
      this.canSendMessage = this.notificationService.connectionExists;  
    }
  }

  isAdmin() : boolean{
    if(localStorage.role == "Admin")
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
