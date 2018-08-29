import { Component, OnInit } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-change-roles',
  templateUrl: './change-roles.component.html',
  styleUrls: ['./change-roles.component.css']
})
export class ChangeRolesComponent implements OnInit {

  constructor(private accountService: AccountService, private router: Router) { }

  users: any;
  roles: Array<String> = ["Admin", "Manager", "AppUser"];

  ngOnInit() {
    this.accountService.getMethodUsers()
    .subscribe(
      res => {
        this.users = res;
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      }
    );
  }

  onChangeRole(userId, role){
    console.log(userId);
    console.log(role);
    this.accountService.putMethodChangeRole(userId, role)
    .subscribe(
      res => {
        alert(res);
        this.router.navigateByUrl("/Vehicles");
      },
      error => {
        alert(error.error.Message);
      }
    );
  }

}
