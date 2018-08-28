import { Injectable } from '@angular/core';

import { HttpClient, HttpHeaders } from '@angular/common/http';

import { Observable } from 'rxjs/Observable';
import 'rxjs/add/operator/catch';
import 'rxjs/add/operator/map';

@Injectable({
  providedIn: 'root'
})
export class CommentService {

  formData: FormData = new FormData();

  constructor(private httpClient: HttpClient) { }

  postMethodCreateComment(comment):Observable<any>{
    console.log("serviceId: " + comment.serviceId);
    console.log("text", comment.text);

    this.formData.append('serviceId',comment.serviceId);
    this.formData.append('text', comment.text);

    let headers = new HttpHeaders();
    headers.append('Content-Type', 'application/json')

    let result =  this.httpClient.post("http://localhost:51680/api/Comments/PostComment", this.formData, { headers: headers }); 
    
    this.formData = new FormData();

    return result;
  }

}
