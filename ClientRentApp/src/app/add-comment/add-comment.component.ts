import { Component, OnInit } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router, ActivatedRoute} from '@angular/router';
import { CommentService } from '../services/comment.service'
import { Comment } from '../models/comment.model';

@Component({
  selector: 'app-add-comment',
  templateUrl: './add-comment.component.html',
  styleUrls: ['./add-comment.component.css']
})

export class AddCommentComponent implements OnInit {

  serviceId: string = "-1";

  constructor(private router: Router, private activatedRoute: ActivatedRoute, private commentService: CommentService) { 
    activatedRoute.params.subscribe(params => {this.serviceId = params["ServiceId"]});
  }

  ngOnInit() {
  }

  onSubmit(form: NgForm, comment: Comment) {

    comment.serviceId = this.serviceId;

    this.commentService.createComment(comment).subscribe(
      data =>{
        console.log(data);
      }, error =>{
        console.log(error);
      });
  }
  
}
