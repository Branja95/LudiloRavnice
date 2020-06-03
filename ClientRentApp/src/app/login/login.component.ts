import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { LoginUser } from '../models/login-user.model';
import { LoginService } from '../services/login.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  providers: [LoginService]
})
export class LoginComponent implements OnInit {

  constructor(private loginService:LoginService, private router: Router) { }

  ngOnInit() {
  }

  onSubmit(form: NgForm, loginUser: LoginUser) {
    if(!localStorage.jwt)
    {
      this.loginService.postMethodLogin(loginUser).subscribe(
        res => {
          //console.log(res)
          this.saveToken(res);
          this.goToDefault();
        },
        err => {
          console.log('1', err)
          alert("Invalid username or/and password.")
        }
      );
    }
  }

  saveToken(data) : void {

    let jwt = data.access_token;
    let jwtData = jwt.split('.')[1]
    let decodedJwtJsonData = window.atob(jwtData)
    let decodedJwtData = JSON.parse(decodedJwtJsonData)
    
    localStorage.setItem('jwt', jwt)
    localStorage.setItem('role', decodedJwtData.role);
    localStorage.setItem('username', decodedJwtData.username);
    localStorage.setItem('nameid', decodedJwtData.nameid);
  
  }

  goToDefault() : void {
    this.router.navigate(['/RentVehicle']);
  }
}
