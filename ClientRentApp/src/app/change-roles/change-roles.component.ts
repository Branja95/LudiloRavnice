import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AccountService } from '../services/account.service';
import { environment } from '../../environments/environment';

@Component({
  selector: 'app-change-roles',
  templateUrl: './change-roles.component.html',
  styleUrls: ['./change-roles.component.css']
})
export class ChangeRolesComponent implements OnInit {

  constructor(private accountService: AccountService, private router: Router) { }

  users: any;
  userImageLoad = environment.endpointAccountGetUserImage;
  roles: Array<String> = ["Administrator", "Manager", "Client"];

  ngOnInit() {
    this.accountService.getMethodUsers().subscribe(
      res => {
        this.users = res;
        this.users.forEach(user => {
          user.DateOfBirth = user.DateOfBirth.split('T', 1)[0]
        });
      },
      error => {
        console.log(error);
    });
  }

  onChange(role, userId){
    var request: any = {};
    request.Role = role;
    this.accountService.putMethodChangeRole(userId, request).subscribe(
      error => {
        console.log(error);
    });
  }
}
