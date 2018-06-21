import { Component, OnInit } from '@angular/core';
import { NgForm, FormsModule } from '@angular/forms';
import {
  Router,
  ActivatedRoute
} from '@angular/router';

import { Vehicle } from '../models/vehicle.model';
import { VehicleType } from '../models/vehicle-type.model';
import { VehicleService } from '../services/vehicle.service';

@Component({
  selector: 'app-edit-vehicle',
  templateUrl: './edit-vehicle.component.html',
  styleUrls: ['./edit-vehicle.component.css']
})
export class EditVehicleComponent implements OnInit {

  VehicleId: string = "-1";
  Vehicle: Vehicle;

  urls: Array<string> = new Array<string>();
  uploadedFiles: FileList = null;
  vehicleTypes = Array<VehicleType>()
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private vehicleService: VehicleService) { 
    activatedRoute.params.subscribe(params => {this.VehicleId = params["VehicleId"]});
  }

  ngOnInit() {
    this.getVehicleTypes();
    this.vehicleService.getVehicle(this.VehicleId).subscribe(
      data => {
        this.Vehicle = data as Vehicle;
      },error => {
        alert(error.error.Message);
      });
  }

  handleFileInput(files: FileList) {
    this.uploadedFiles = files;
    
    let fileList = Array.from(files);
    
    if (fileList) {
      for (let file of fileList) {
        let reader = new FileReader();
        reader.onload = (e: any) => {
          this.urls.push(e.target.result);
        }
        reader.readAsDataURL(file);
      }
    }
  }

  onSubmit(form: NgForm) {
    this.vehicleService.editVehicle(this.VehicleId, this.Vehicle, this.uploadedFiles)
    .subscribe(
      data => {
        alert(data);
      }, error => {
        alert(error);
      });;
    
      form.reset();
      this.urls = new Array<string>();
      this.uploadedFiles = null;
      this.router.navigateByUrl('Vehicle');
  }

  getVehicleTypes() { 
    this.vehicleService.getVehicleTypes()
    .subscribe(
      res => { 
        this.vehicleTypes = res as Array<VehicleType> 
    });  
  }

  parseImages(imageId){
    return imageId.split(";_;");
  }

}
