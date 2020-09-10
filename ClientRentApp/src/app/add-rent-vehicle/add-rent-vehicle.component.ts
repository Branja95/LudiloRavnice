import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { RentVehicle } from '../models/rent-vehicle.model';
import { RentVehicleService } from '../services/rent-vehicle.service';

@Component({
  selector: 'app-add-rent-vehicle',
  templateUrl: './add-rent-vehicle.component.html',
  styleUrls: ['./add-rent-vehicle.component.css']
})
export class AddRentVehicleComponent implements OnInit {

  url: string = '';
  file: File = null;
  
  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) { }

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

  onSubmit(form: NgForm, rentVehicle: RentVehicle) {
    
    this.rentVehicleService.postMethodCreateRentVehicleService(rentVehicle, this.file)
    .subscribe(
      res => {
      }, error => {
        console.log(error);
      });;
    
    form.reset();
    this.url = '';
    this.file = null;
  }

}
