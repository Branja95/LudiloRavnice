import { Component, OnInit } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import {Router, ActivatedRoute } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-edit-rent-vehicle',
  templateUrl: './edit-rent-vehicle.component.html',
  styleUrls: ['./edit-rent-vehicle.component.css'],
  providers: [RentVehicleService]
})
export class EditRentVehicleComponent implements OnInit {

  ServiceId: string = "-1";
  rentVehicle: any;
    
  selecetdFileUrl: string = '';
  selectedFile: File = null;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {this.ServiceId = params["ServiceId"]});
  }

  ngOnInit() {
    this.rentVehicleService.getService(this.ServiceId).subscribe(
      res => {
        this.rentVehicle = res;
        console.log(this.rentVehicle);
      },error => {
        console.log(error);
      });
  }

  handleFileInput(event) {
    this.selectedFile = event.target.files[0];
    
    if (event.target.files && event.target.files[0]) {
      var reader = new FileReader();

      reader.readAsDataURL(event.target.files[0]); 
      reader.onload = (event) => { 
        this.selecetdFileUrl = reader.result as string;
      }
    }
  }

  onSubmit(form: NgForm) {
    console.log('eee2')
    this.rentVehicleService.editService(this.ServiceId, this.rentVehicle, this.selectedFile)
    .subscribe(
      res => {
        console.log(res);
      }, error => {
        console.log(error);
      });;
    
      form.reset();
      this.selecetdFileUrl = '';
      this.selectedFile = null;
      this.router.navigateByUrl('Service');
    
  }

}
