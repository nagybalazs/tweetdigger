import { Component, OnInit } from '@angular/core';
import { Channel } from '../../classes/classes';
import { ChannelService } from '../../services/services';
import { Http } from '@angular/http';

@Component({
    templateUrl: 'home.component.html',
    styleUrls: ['home.component.css']
})
export class HomeComponent implements OnInit {

    channels: Channel[];

    constructor(private http: Http) { }

    ngOnInit() {
        this.channels = ChannelService.getChannels();
    }

}