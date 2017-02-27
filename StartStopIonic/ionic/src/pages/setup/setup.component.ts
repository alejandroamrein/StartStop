import {Component} from '@angular/core';
import {NavController} from 'ionic-angular'; 
import {FormBuilder, /*FormGroup,*/ Validators} from '@angular/forms';

import {LoginComponent} from '../login/login.component';

import {DataService} from '../../services/data';

@Component({
    templateUrl: 'setup.component.html'
})
export class SetupComponent {

    public setupForm: any;
    public url;

    constructor(private nav: NavController,
        private _form: FormBuilder,
        private dataService: DataService) {
        this.url = dataService.getServiceUrl();
        this.setupForm = this._form.group({
            'url': ["", Validators.required]
        });
    }

    submit() {
        this.dataService.ping(this.url).subscribe(
            data => {
                var response = data.json();
                if (response == "ok") {
                    this.dataService.setServiceUrl(this.url);
                    this.nav.setRoot(LoginComponent);
                }
            },
            err => {
                console.error(err);
                alert("URL nicht erreichbar");
            },
            () => {
                console.log('Setup completed');
            }
        );
    }
}