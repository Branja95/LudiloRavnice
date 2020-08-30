import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';
import { RentVehicleService } from '../services/rent-vehicle.service';
import { CommentService } from '../services/comment.service';
import { Comment} from '../models/comment.model'

@Component({
  selector: 'app-comment',
  templateUrl: './comment.component.html',
  styleUrls: ['./comment.component.css']
})

export class CommentComponent implements OnInit {

  checkoutForm;
  serviceId: string = "-1";

  canUserLeaveFeedback: boolean;
  comments: Array<Comment>;

  constructor(private router: Router, private formBuilder: FormBuilder, private activatedRoute: ActivatedRoute, private rentVehicleService: RentVehicleService, private commentService: CommentService) {
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
    this.checkoutForm = this.formBuilder.group({
      text: '',
      value: 1
    });
  }

  ngOnInit() {
    this.rentVehicleService.getMethodComments(this.serviceId).subscribe(
      res => {
        this.comments = res as Array<Comment>;
        this.comments.forEach(comment => {
          comment.DateTime = comment.DateTime.split('T', 1)[0]
        });
      },error =>{
        console.log(error);
      });

    this.rentVehicleService.getMethodHasUserCommented(this.serviceId).subscribe(
      res => {
        this.canUserLeaveFeedback = res as boolean;
      }, error =>{
        console.log(error);
      });
  }
  
  canUserPostFeedback() : boolean {
    if(this.canUserLeaveFeedback)
    {
      return true;
    }
    else
    {
      return false;
    }
  }

  onSubmit(data) {
    data.serviceId = this.serviceId;
    this.commentService.postMethodCreateComment(data).subscribe(
      res =>{
        this.ngOnInit();
      }, 
      error =>{
        console.log(error);
      });
  }
}
