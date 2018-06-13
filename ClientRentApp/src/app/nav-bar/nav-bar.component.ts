import { Component, OnInit } from '@angular/core';

import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { LogoutService } from '../services/logout.service';

@Component({
  selector: 'app-nav-bar',
  templateUrl: './nav-bar.component.html',
  styleUrls: ['./nav-bar.component.css'],
  providers: [LogoutService]
})
export class NavBarComponent implements OnInit {

  constructor(private logoutService: LogoutService, private router: Router) { }

  ngOnInit() {
  }

  isLogged()
  {
    if(!localStorage.jwt)
    {
      return false;
    }
    else
    {
      return true;
    }
  }

  logoutUser()
  {
    this.logoutService.postMethodLogout().subscribe(
      res => {
        console.log(res);
      },
      error => {
        alert(error.error.Message);
      })
    localStorage.clear();
    this.router.navigate(['/BranchOffice']);
  }

}
