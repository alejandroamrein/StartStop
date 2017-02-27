import { Component } from '@angular/core';
import { ModalController, ViewController, NavParams } from 'ionic-angular'; 

@Component({
    templateUrl: 'home.history.dialog.html'
})
export class HistoryDialog {

    public items: any[] = [];

    constructor(private modal: ModalController,
        public view: ViewController,
        public navParams: NavParams) {
        this.items = this.navParams.get('items');
        console.log(this.items);
    }

    dismiss() {
        this.view.dismiss();
    }
}
