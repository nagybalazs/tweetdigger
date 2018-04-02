import { Tweet } from "./tweet.class";
import { StoredChannel } from './storedchannel.class';

export class Channel extends StoredChannel { 

    tweets: Tweet[];

    constructor() {
        super();
        this.tweets = new Array<Tweet>();
    }

    public toStoredChannel(): StoredChannel {
        return { closed: this.closed, column: this.column, endpoint: this.endpoint, name: this.name };
    }
}