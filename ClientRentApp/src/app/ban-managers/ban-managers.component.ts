import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-ban-managers',
  templateUrl: './ban-managers.component.html',
  styleUrls: ['./ban-managers.component.css']
})
export class BanManagersComponent implements OnInit {

  managers: any;

  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit() {
    this.accountService.getMethodManagers()
    .subscribe(
      res => {
        this.managers = res;
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      }
    );
  }

  changeBan(managerId){
    console.log(managerId);
    this.accountService.putMethodManagers(managerId)
    .subscribe(
      res => {
        this.managers = res;
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      }
    );
  }

}
