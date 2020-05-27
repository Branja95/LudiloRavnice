import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { RegistrationService } from '../services/registration.service';
import { NgForm } from '@angular/forms';

@Component({
  selector: 'app-approve-account',
  templateUrl: './approve-account.component.html',
  styleUrls: ['./approve-account.component.css'],
  providers: [RegistrationService]
})
export class ApproveAccountComponent implements OnInit {

  url: string = '';
  file: File = null;

  constructor(private registrationService: RegistrationService, private router: Router) { }

  ngOnInit() {
  }

  handleFileInput(event) {
    this.file = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 
      reader.onload = (event) => { 
        this.url = reader.result as string;
      }
    }
  }

  onSubmit(form: NgForm) {
    if(this.file != null){
      this.registrationService.postMethodApproveAccount(this.file).subscribe(
        res => {
          this.router.navigate(['/RentVehicle']);
        }, 
        error => {
          console.log(error)
          alert(error.message);
      });

      form.reset();
      this.url = '';
      this.file = null;
    }
  }

}
