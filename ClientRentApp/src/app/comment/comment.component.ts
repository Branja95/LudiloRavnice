import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { Comment} from '../models/comment.model'
import { Observable } from 'rxjs';

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})

export class CommentComponent implements OnInit {

  serviceId: string = "-1";

  userCommented: boolean;
  comments: Array<Comment>;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
    this.rentVehicleService.getMethodComments(this.serviceId).subscribe(
      res => {
        this.comments = res as Array<Comment>;
        console.log(this.comments);
      }, 
      error =>{
        console.log(error);
      });

    this.rentVehicleService.getMethodHasUserCommented(this.serviceId).subscribe(
      res => {
        this.userCommented = res as boolean;
      }, error =>{
        console.log(error);
      });
  }
  
  hasUserCommented() : boolean {
    if(this.userCommented)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

}
