import { Component, OnInit } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { RentVehicle } from '../models/rent-vehicle.model';
import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-edit-rent-vehicle',
  templateUrl: './edit-rent-vehicle.component.html',
  styleUrls: ['./edit-rent-vehicle.component.css']
})
export class EditRentVehicleComponent implements OnInit {

  ServiceId: string = "-1";
  RentVehicle: RentVehicle;
    
  selecetdFileUrl: string = '';
  selectedFile: File = null;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {this.ServiceId = params["ServiceId"]});
  }

  ngOnInit() {
    this.rentVehicleService.getService(this.ServiceId).subscribe(
      data => {
        this.RentVehicle = data as RentVehicle;
        console.log(this.RentVehicle);
      },error => {
        alert(error.error.Message);
      });
  }

  handleFileInput(event) {

    this.selectedFile = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 

      reader.onload = (event) => { 
        this.selecetdFileUrl = reader.result;
      }
    }

  }

  onSubmit(form: NgForm) {
  
    this.rentVehicleService.editService(this.ServiceId, this.RentVehicle, this.selectedFile)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        alert(error);
      });;
    
      form.reset();
      this.selecetdFileUrl = '';
      this.selectedFile = null;
      this.router.navigateByUrl('Service');
    
  }

}
