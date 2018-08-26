import { Component, OnInit } from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { Comment} from '../models/comment.model'

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})

export class CommentComponent implements OnInit {

  serviceId: string = "-1";
  comments: Array<Comment>;

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService) {
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
    /* this.rentVehicleService.getComments(this.serviceId).subscribe(
      res => {
        this.comments = res as Array<Comment>;
        console.log(this.comments);
      }, 
      error =>{
        console.log(error);
      }); */
  }

}
