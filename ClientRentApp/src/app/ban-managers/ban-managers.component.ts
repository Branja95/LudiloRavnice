import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-ban-managers',
  templateUrl: './ban-managers.component.html',
  styleUrls: ['./ban-managers.component.css']
})
export class BanManagersComponent implements OnInit {

  managers: any;
  userImageLoad = environment.endpointAccountGetUserImage;

  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit() {
    this.accountService.getMethodManagers()
    .subscribe(
      res => {
        this.managers = res;
        this.managers.forEach(manager => {
          manager.DateOfBirth = manager.DateOfBirth.split('T', 1)[0]
        });
      },
      error => {
        console.log(error);
      }
    );
  }

  changeBan(managerId){
    this.accountService.putMethodManagers(managerId)
    .subscribe(
      res => {
        this.managers = res;
        this.managers.forEach(manager => {
          manager.DateOfBirth = manager.DateOfBirth.split('T', 1)[0]
        });
      },
      error => {
        console.log(error);
      }
    );
  }

}
