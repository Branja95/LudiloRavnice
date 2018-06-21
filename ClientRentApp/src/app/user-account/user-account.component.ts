import { Component, OnInit } from '@angular/core';

import { NavBarService } from '../services/nav-bar.service';

@Component({
  selector: 'app-user-account',
  templateUrl: './user-account.component.html',
  styleUrls: ['./user-account.component.css'],
  providers: [NavBarService]
})
export class UserAccountComponent implements OnInit {

  private approved : boolean;

  constructor(private navBarService: NavBarService) { }

  ngOnInit() {
    this.navBarService.getMethodIsUserApproved()
    .subscribe(
      res => {
        this.approved = res;
      },
      error => {
        localStorage.clear();
        console.log(error.error.Message);
      })
      this.approved = true;
  }

  isApproved() : boolean {
    return this.approved;
  }

}
