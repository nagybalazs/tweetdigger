import { Component, OnInit, Input } from '@angular/core';
import { HubConnection } from '@aspnet/signalr-client';
import { Tweet } from '../../classes/classes';

@Component({
    selector: 'channel',
    templateUrl: './channel.component.html'
})
export class ChannelComponent implements OnInit {

    @Input()
    endpoint: string;

    public tweets: Tweet[] = [];
    private _hubConnetion: HubConnection;

    constructor() { }

    ngOnInit() {
        this._hubConnetion = new HubConnection('/' + this.endpoint);

        this._hubConnetion.on('Send', (data: any) => {
            if (this.tweets.length >= 100) {
                this.tweets.shift();
            }
            let tweet = Tweet.create(data);
            this.tweets.unshift(tweet);
            console.log(JSON.stringify(tweet));
        });

        this._hubConnetion.start()
            .then(() => {
                console.log('started');
            })
            .catch(err => {
                debugger;
                console.log(JSON.stringify(err));
            });
    }

}
