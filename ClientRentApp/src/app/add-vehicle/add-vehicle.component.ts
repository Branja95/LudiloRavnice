import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { Vehicle } from '../models/vehicle.model';
import { VehicleService } from '../services/vehicle.service';
import { VehicleType } from '../models/vehicle-type.model';

@Component({
  selector: 'app-add-vehicle',
  templateUrl: './add-vehicle.component.html',
  styleUrls: ['./add-vehicle.component.css']
})
export class AddVehicleComponent implements OnInit {
  ServiceId: string = "-1";

  urls: Array<string> = new Array<string>();
  uploadedFiles: FileList = null;
  vehicleTypes = Array<VehicleType>()

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private vehicleService: VehicleService) {
    activatedRoute.params.subscribe(params => {this.ServiceId = params["ServiceId"]});
  }

  ngOnInit() {
    this.getVehicleTypes()
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

  onSubmit(form: NgForm, vehicle: Vehicle) {
    vehicle.serviceId = this.ServiceId;

    this.vehicleService.createVehicle(vehicle, this.uploadedFiles).subscribe(
      res => {
        alert(res);
      }, 
      error => {
        alert(error.error.Message);
    });

    form.resetForm();
    this.urls = new Array<string>();
    this.getVehicleTypes();
  }
  
  getVehicleTypes() { 
    this.vehicleService.getVehicleTypes().subscribe(res => { this.vehicleTypes = res as Array<VehicleType> });  
  }
  
}
