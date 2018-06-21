import { Component, OnInit } from '@angular/core';

import { Router, ActivatedRoute } from '@angular/router';

import { AccountService } from '../services/account.service';

@Component({
  selector: 'app-approve-account-admin',
  templateUrl: './approve-account-admin.component.html',
  styleUrls: ['./approve-account-admin.component.css'],
  providers: [AccountService]
})
export class ApproveAccountAdminComponent implements OnInit {

  private users : any;

  constructor(private accountService: AccountService, private router: Router) { }

  ngOnInit() {
    this.accountService.getMethodUsersForApproves()
    .subscribe(
      res => {
        this.users = res;
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      })
  }

  approveUserAccount(id : number)
  {
    this.accountService.postMethodApproveUser(id)
    .subscribe(
      res => {
        console.log(res);
        this.router.navigate(['/ApproveAccountAdmin']);
      },
      error => {
        alert(error.error.Message);
      })
  }

  rejectUserAccount(id : number)
  {
    this.accountService.postMethodRejectUser(id)
    .subscribe(
      res => {
        console.log(res);
        this.router.navigate(['/ApproveAccountAdmin']);
      },
      error => {
        alert(error.error.Message);
      })
  }

}
