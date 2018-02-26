import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { AddAccountResultType } from '../classes/classes';
import { Observable } from 'rxjs/Observable';
import 'rxjs'

@Injectable()
export class AccountService {

    constructor(private http: HttpClient) { }

    signup(email: string): Observable<{ result: AddAccountResultType }> {
        let body: { email: string } = { email: email };
        return this.http.post<{ result: AddAccountResultType }>("/api/account/signup", body).map(res => {
            console.log(JSON.stringify(res));
            return res;
        });
    }

}