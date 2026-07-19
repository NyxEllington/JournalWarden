# Event Type Categories

Common Elite Dangerous event types that users frequently filter:

## Music & Audio Events
- **Music** - Background music changes
- **StartJump** - FSD charging sound
- **ReceiveText** - Incoming messages (can be spammy)

## Friend & Social Events  
- **Friends** - Friend list updates
- **FriendsStatus** - Friend online/offline status
- **Commander** - Commander info updates

## File & Session Events
- **Fileheader** - Journal file metadata (appears at start of each file)
- **LoadGame** - Game loaded (appears frequently during testing)
- **Shutdown** - Game shutdown event

## Status & Heartbeat Events
- **Status** - Status.json updates (very frequent)
- **Cargo** - Cargo hold updates
- **ModuleInfo** - Ship module information
- **Outfitting** - Outfitting screen opened
- **Shipyard** - Shipyard screen opened
- **Market** - Market screen opened

## Screenshot Events
- **Screenshot** - Screenshots taken (can be frequent for explorers)

## Repetitive Navigation
- **ApproachBody** - Approaching a body
- **LeaveBody** - Leaving a body  
- **SupercruiseEntry** - Entered supercruise
- **SupercruiseExit** - Dropped from supercruise

## Commonly Filtered Combinations

### "Quiet Mode" - Hide noise during exploration:
- Music
- ReceiveText
- Status
- Screenshot
- ApproachBody
- LeaveBody

### "Trading Focus" - Show only trade-relevant events:
Hide everything except:
- Docked
- Undocked
- MarketBuy
- MarketSell
- CommodityPricesUpdate

### "Combat Focus" - Show only combat events:
Hide everything except:
- ShipTargeted
- UnderAttack
- HullDamage
- ShieldsUp
- ShieldsDown
- Interdicted
- Died
- PVPKill

### "Jump Tracking" - Show only system changes:
Hide everything except:
- FSDJump
- Location
- Docked
- Undocked
