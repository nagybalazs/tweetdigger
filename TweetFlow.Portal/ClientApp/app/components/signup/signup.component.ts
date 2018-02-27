import { Component } from '@angular/core';
import { AccountService } from '../../services/services';
import { AddAccountResultType } from '../../classes/classes';
import 'rxjs';

@Component({
    templateUrl: 'signup.component.html',
    styleUrls: ['signup.component.css'],
    selector: 'sign-up'
})
export class SignupComponent {

    constructor(private accountService: AccountService) { }

    email: string = "";

    showForm: boolean = true;
    showError: boolean = false;
    showTaken: boolean = false;
    showSuccess: boolean = false;
    showEmpty: boolean = false;

    signup() {
        this.showEmpty = false;
        if (!this.email) {
            this.showEmpty = true;
            return;
        }
        this.accountService.signup(this.email)
            .subscribe(result => {
                switch (result.result) {
                    case AddAccountResultType.AlreadyExists: {
                        this.showTaken = true;
                        break;
                    };
                    case AddAccountResultType.EmailEmpty: {
                        this.showError = true;
                        break;
                    };
                    case AddAccountResultType.Success: {
                        this.showForm = false;
                        this.showSuccess = true;
                        break;
                    };
                    case AddAccountResultType.Unknown: {
                        this.showError = true;
                        break;
                    }
                }
            }, err => {

            });
        
    }

}