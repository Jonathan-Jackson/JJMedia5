<template>
  <div class="setup layout-content p-grid">
    <div class="p-col" />
    <Card class="p-col">
      <template #title>
        Setup
      </template>
      <template #content>
        {{ getText() }}
      </template>
      <template #footer>
        <div id="UserInput" v-if="isAwaitingUser()">
          <label for="Username" class="p-sr-only">Username</label>
          <InputText
            id="Username"
            type="text"
            placeholder="Username"
            :value="username"
          />
          <Button type="button" label="Next" @click="addUser" />
        </div>
        <div id="RssInput" v-else-if="isAwaitingRss()">
          <DataTable :value="rss" responsiveLayout="scroll">
            <Column field="url" header="Address"></Column>
            <Column field="isSubscribed" header="Subscribed"></Column>
            <Column field="regexMatch" header="Regex Match"></Column>
          </DataTable>
          <Button type="button" label="Back" @click="previousTab" />
          <Button type="button" label="Next" @click="nextTab" />
        </div>
        <div id="TClientInput" v-else-if="isAwaitingTClient()">
          <label for="Address" class="p-sr-only">Address</label>
          <InputText
            id="Address"
            type="text"
            placeholder="Address"
            value="localhost:8080"
          />
          <label for="Username" class="p-sr-only">Username</label>
          <InputText id="Username" type="text" placeholder="Username" />
          <label for="Password" class="p-sr-only">Password</label>
          <InputText id="Password" type="text" placeholder="Password" />
          <Button type="button" label="Back" @click="previousTab" />
          <Button type="button" label="Test" @click="testConnection" />
          <Button type="button" label="Next" @click="addTClient" />
        </div>
        <div id="ImportsInput" v-else-if="isAwaitingImports()">
          <DataTable :value="imports" responsiveLayout="scroll">
            <Column field="folder" header="Folder Location"></Column>
            <Column field="isPolled" header="Automatic Checks"></Column>
            <Column field="isActive" header="Active"></Column>
          </DataTable>
          <Button type="button" label="Back" @click="previousTab" />
          <Button type="button" label="Next" @click="nextTab" />
        </div>
        <div id="StoresInput" v-else-if="isAwaitingStores()">
          <DataTable :value="stores" responsiveLayout="scroll">
            <Column field="folder" header="Folder Location"></Column>
            <Column field="priority" header="Priority"></Column>
            <Column field="isActive" header="Active"></Column>
          </DataTable>
          <Button type="button" label="Back" @click="previousTab" />
          <Button type="button" label="Finish" @click="finish" />
        </div>
      </template>
    </Card>
    <div class="p-col" />
  </div>
</template>

<script>
export default {
  data: function() {
    return {
      tab: 0,
      username: "",
      rss: [
        {
          url: "https://subsplease.org/rss/?t&r=1080",
          isSubscribed: true,
          regexMatch: "",
        },
      ],
      tclient: null,
      imports: [{ folder: "G:\\JJDownloads", isPolled: true, isActive: true }],
      stores: [{ folder: "G:\\JJStore", priority: 1, isActive: true }],
    };
  },
  methods: {
    getText: function() {
      if (this.isAwaitingUser()) {
        return "Create an admin user for access";
      } else if (this.isAwaitingRss()) {
        return "Enter an RSS Feed for automatically adding new media. The address should directly point at an RSS XML document. The Regex Match is a pattern to match on for the item to be eligible for download, leave this as empty if you wish to download all items.";
      } else if (this.isAwaitingTClient()) {
        return "Enter your torrent clients API credentials to push feeds into, currently this only supports QBittorrent!";
      } else if (this.isAwaitingImports()) {
        return "Which folder(s) are you importing files into your media store from? This should generally be your downloads location from your torrent client for media.";
      } else if (this.isAwaitingStores()) {
        return "Where are you storing your media? The first store with space available is used.";
      }
      return "";
    },
    isAwaitingUser: function() {
      return this.tab === 0 || this.username === "";
    },
    isAwaitingRss: function() {
      return (
        (this.tab === 1 || this.rss.length === 0) && !this.isAwaitingUser()
      );
    },
    isAwaitingTClient: function() {
      return (this.tab === 2 || this.tclient === null) && !this.isAwaitingRss();
    },
    isAwaitingImports: function() {
      return (
        (this.tab === 3 || this.imports.length === 0) &&
        !this.isAwaitingTClient()
      );
    },
    isAwaitingStores: function() {
      return (
        (this.tab === 4 || this.stores.length === 0) &&
        !this.isAwaitingImports()
      );
    },
    addUser: function() {
      this.username = document.getElementById("Username").value;
      this.nextTab();
    },
    addRss: function() {
      const newRss = {};
      this.rss.push(newRss);
      this.nextTab();
    },
    nextTab: function() {
      this.tab++;
    },
    previousTab: function() {
      this.tab--;
    },
    addTClient: function() {
      const address = document.getElementById("Address").value;
      const username = document.getElementById("Username").value;
      const password = document.getElementById("Password").value;

      this.tclient = {
        address,
        username,
        password,
      };
      this.nextTab();
    },
    testConnection: function() {
      const address = document.getElementById("Address").value;
      const username = document.getElementById("Username").value;
      const password = document.getElementById("Password").value;

      console.log(
        `testing connection: address - ${address} username - ${username} password - ${password}`
      );
    },
    createSetupModel: function() {
      return {
        username: this.username,
        rss: this.rss,
        torrentClient: this.tclient,
        imports: this.imports,
        stores: this.stores,
      };
    },
    finish: function() {
      console.log("finish called");
      console.log(JSON.stringify(this.createSetupModel()));
    },
  },
};
</script>
