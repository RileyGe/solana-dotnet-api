using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using NokitaKaze.Base58Check;

namespace Solana
{
    /// <summary>
    /// Account metadata used to define instructions
    /// </summary>
    public struct AccountMeta {
        /// <summary>
        /// An account's public key
        /// </summary>
        public PublicKey pubkey;
        /// <summary>
        /// True if an instruction requires a transaction signature matching `pubkey`
        /// </summary>
        public bool isSigner;
        /// <summary>
        /// True if the `pubkey` can be loaded as a read-write account.
        /// </summary>
        public bool isWritable;
    }



    /// <summary>
    /// List of TransactionInstruction object fields that may be initialized at construction
    /// </summary>
    public struct TransactionInstructionCtorFields {
        public List<AccountMeta> keys;
        public PublicKey programId;
        public byte[] data;
    }
    /// <summary>
    /// Transaction Instruction class
    /// </summary>
    public class TransactionInstruction
    {
        /// <summary>
        /// Public keys to include in this transaction
        /// Boolean represents whether this pubkey needs to sign the transaction
        /// </summary>
        public List<AccountMeta> keys;

        /// <summary>
        /// Program Id to execute
        /// </summary>
        public PublicKey programId;

        /// <summary>
        /// Program input
        /// </summary>
        public byte[] data = new byte[0];

        public TransactionInstruction(TransactionInstructionCtorFields opts)
        {
            this.programId = opts.programId;
            this.keys = opts.keys;
            if (opts.data.Length > 0)
            {
                this.data = opts.data;
            }
        }
    }

    /// <summary>
    /// Pair of signature and corresponding public key
    /// </summary>
    public struct SignaturePubkeyPair {
        public byte[] signature;
        public PublicKey publicKey;
    }


    /// <summary>
    /// Nonce information to be used to build an offline Transaction.
    /// </summary>
    public class NonceInformation {
        /// <summary>
        /// The current blockhash stored in the nonce
        /// </summary>
        public string nonce;
        /// <summary>
        /// AdvanceNonceAccount Instruction
        /// </summary>
        public TransactionInstruction nonceInstruction;
    }

public class Transaction
    {
        public List<SignaturePubkeyPair> signatures;
        public Transaction()
        {
            instructions = new List<TransactionInstruction>();
            signatures = new List<SignaturePubkeyPair>();
        }

        /// <summary>
        /// Sign the Transaction with the specified accounts. Multiple signatures may
        /// be applied to a Transaction. The first signature is considered "primary"
        /// and is used identify and confirm transactions.
        ///
        /// If the Transaction `feePayer` is not set, the first signer will be used
        /// as the transaction fee payer account.
        ///
        /// Transaction fields should not be modified after the first call to `sign`,
        /// as doing so may invalidate the signature and cause the Transaction to be
        /// rejected.
        ///
        /// The Transaction must be assigned a valid `recentBlockhash` before invoking this method
        /// </summary>
        /// <param name="signers"></param>
        public void Sign(Account[] signers)
        {
            if(signers.Length == 0)
            {
                throw new ArgumentException("No signers");
            }

            // Dedupe signers
            var seen = new HashSet<string>();
            var uniqueSigners = new List<Account>();
            foreach (var signer in signers) {
                var key = signer.PublicKey.ToString();
                if (seen.Contains(key))
                {
                    continue;
                }
                else
                {
                    seen.Add(key);
                    uniqueSigners.Add(signer);
                }
            }

            uniqueSigners.ForEach(signer => {
                signatures.Add(new SignaturePubkeyPair()
                {
                    signature = null,
                    publicKey = signer.PublicKey
                });
            });

            var message = this._compile();
            this._partialSign(message, uniqueSigners.ToArray());
            //        this._verifySignatures(message.serialize(), true);

        }




        //        /**
        //   * Signatures for the transaction.  Typically created by invoking the
        //   * `sign()` method
        //   */
        //        signatures: Array<SignaturePubkeyPair> = [];

        //  /**
        //   * The first (payer) Transaction signature
        //   */
        //  get signature(): Buffer | null {
        //    if (this.signatures.length > 0) {
        //            return this.signatures[0].signature;
        //        }
        //    return null;
        //        }

        /// <summary>
        /// The transaction fee payer
        /// </summary>
        PublicKey feePayer;

        /// <summary>
        /// The instructions to atomically execute
        /// </summary>
        List<TransactionInstruction> instructions;

        /// <summary>
        /// A recent transaction id. Must be populated by the caller
        /// </summary>
        string recentBlockhash;

        /// <summary>
        /// Optional Nonce information. If populated, transaction will use a durable
        /// Nonce hash instead of a recentBlockhash.Must be populated by the caller
        /// </summary>
        NonceInformation nonceInfo;

        //  /**
        //   * Construct an empty Transaction
        //   */
        //  constructor(opts?: TransactionCtorFields)
        //        {
        //            opts && Object.assign(this, opts);
        //        }

        //        /**
        //         * Add one or more instructions to this Transaction
        //         */
        //        add(
        //    ...items: Array<
        //            Transaction | TransactionInstruction | TransactionInstructionCtorFields
        //          >

        //        ) : Transaction {
        //    if (items.length === 0) {
        //      throw new Error('No instructions');
        //    }

        //    items.forEach((item: any) => {
        //      if ('instructions' in item) {
        //        this.instructions = this.instructions.concat(item.instructions);
        //      } else if ('data' in item && 'programId' in item && 'keys' in item) {
        //        this.instructions.push(item);
        //      } else {
        //        this.instructions.push(new TransactionInstruction(item));
        //      }
        //    });
        //return this;
        //  }

        /// <summary>
        /// Compile transaction data
        /// </summary>
        /// <returns></returns>
        public Message compileMessage()
        {
            var nonceInfo = this.nonceInfo;
            if (nonceInfo != null && this.instructions[0] != nonceInfo.nonceInstruction)
            {
                this.recentBlockhash = nonceInfo.nonce;
                this.instructions.Insert(0, nonceInfo.nonceInstruction);
            }
            var recentBlockhash = this.recentBlockhash;
            if (recentBlockhash.Length < 1)
            {
                throw new Exception("Transaction recentBlockhash required");
            }

            if (this.instructions.Count < 1)
            {
                throw new Exception("No instructions provided");
            }

            PublicKey feePayer;
            if (this.feePayer != null)
            {
                feePayer = this.feePayer;
            }
            else if (signatures.Count > 0 && signatures[0].publicKey != null)
            {
                // Use implicit fee payer
                feePayer = this.signatures[0].publicKey;
            }
            else
            {
                throw new Exception("Transaction fee payer required");
            }

            for (var i = 0; i < this.instructions.Count(); i++)
            {
                if (this.instructions[i].programId == null)
                {
                    throw new Exception(string.Format("Transaction instruction index {0} has undefined program id", i));
                }
            }

            List<string> programIds = new List<string>();
            List<AccountMeta> accountMetas = new List<AccountMeta>();
            this.instructions.ForEach(instruction => {
                instruction.keys.ForEach(accountMeta =>
                {
                    accountMetas.Add(accountMeta);
                });

                var programId = instruction.programId.ToString();
                if (!programIds.Contains(programId))
                {
                    programIds.Add(programId);
                }
            });

            // Append programID account metas
            programIds.ForEach(programId => {
                accountMetas.Add(new AccountMeta()
                {
                    pubkey = new PublicKey(programId),
                    isSigner = false,
                    isWritable = false,
                });
            });

            // Sort. Prioritizing first by signer, then by writable
            accountMetas.Sort((x, y) => {
                var checkSigner = x.isSigner == y.isSigner ? 0 : x.isSigner ? -1 : 1;
                var checkWritable = x.isWritable == y.isWritable ? 0 : x.isWritable ? -1 : 1;
                return Convert.ToBoolean(checkSigner) ? checkSigner : checkWritable;
            });

            // Cull duplicate account metas
            List<AccountMeta> uniqueMetas = new List<AccountMeta>();
            accountMetas.ForEach(accountMeta =>
            {
                var pubkeyString = accountMeta.pubkey.ToString();
                var uniqueIndex = uniqueMetas.FindIndex(x =>
                {
                    return x.pubkey.ToString() == pubkeyString;
                });
                if (uniqueIndex > -1)
                {
                    var meta = uniqueMetas[uniqueIndex];
                    meta.isWritable = meta.isWritable || accountMeta.isWritable;
                    uniqueMetas[uniqueIndex] = meta;
                }
                else
                {
                    uniqueMetas.Add(accountMeta);
                }
            });

            // Move fee payer to the front
            var feePayerIndex = uniqueMetas.FindIndex(x =>
            {
                return x.pubkey.Equals(feePayer);
            });
            if (feePayerIndex > -1)
            {
                var payerMeta = uniqueMetas[feePayerIndex];
                payerMeta.isSigner = true;
                payerMeta.isWritable = true;
                uniqueMetas.Insert(0, payerMeta);
            }
            else
            {
                uniqueMetas.Insert(0, new AccountMeta()
                {
                    pubkey = feePayer,
                    isSigner = true,
                    isWritable = true,
                });
            }

            // Disallow unknown signers
            foreach (var signature in this.signatures) {
                var uniqueIndex = uniqueMetas.FindIndex(x =>
                {
                    return x.pubkey.Equals(signature.publicKey);
                });
                if (uniqueIndex > -1)
                {
                    if (!uniqueMetas[uniqueIndex].isSigner)
                    {
                        var meta = uniqueMetas[uniqueIndex];
                        meta.isSigner = true;
                        uniqueMetas[uniqueIndex] = meta;
                        //console.warn(
                        //  'Transaction references a signature that is unnecessary, ' +
                        //    'only the fee payer and instruction signer accounts should sign a transaction. ' +
                        //    'This behavior is deprecated and will throw an error in the next major version release.',
                        //);
                    }
                }
                else
                {
                    throw new Exception(string.Format("unknown signer: {0}", signature.publicKey.ToString()));
                }
            }

            var numRequiredSignatures = 0;
            var numReadonlySignedAccounts = 0;
            var numReadonlyUnsignedAccounts = 0;

            // Split out signing from non-signing keys and count header values
            List<string> signedKeys = new List<string>();
            List<string> unsignedKeys = new List<string>();
            uniqueMetas.ForEach(meta => {
                if (meta.isSigner)
                {
                    signedKeys.Add(meta.pubkey.ToString());
                    numRequiredSignatures += 1;
                    if (!meta.isWritable)
                    {
                        numReadonlySignedAccounts += 1;
                    }
                }
                else
                {
                    unsignedKeys.Add(meta.pubkey.ToString());
                    if (!meta.isWritable)
                    {
                        numReadonlyUnsignedAccounts += 1;
                    }
                }
            });

            signedKeys.AddRange(unsignedKeys);
            var accountKeys = signedKeys;
            var instructions = this.instructions.Select(
              instruction =>
              {
                  var data = instruction.data;
                  var programId = instruction.programId;
                  //const { data, programId} = instruction;
                  var cin = new CompiledInstruction();
                  cin.programIdIndex = accountKeys.IndexOf(programId.ToString());
                  cin.accounts = instruction.keys.Select(meta => accountKeys.IndexOf(meta.pubkey.ToString())).ToArray();
                  cin.data = Base58CheckEncoding.EncodePlain(data);
                  return cin;
              }).ToList();

            instructions.ForEach(instruction =>
            {
                if (!(instruction.programIdIndex >= 0))
                {
                    throw new Exception("instruction.programIdIndex error");
                }
                foreach (var keyIndex in instruction.accounts)
                {
                    if (keyIndex < 0)
                    {
                        throw new Exception("instruction.accounts error");
                    }
                }
            });
            var args = new MessageArgs()
            {
                header = new MessageHeader()
                {
                    numRequiredSignatures = numRequiredSignatures,
                    numReadonlySignedAccounts = numReadonlySignedAccounts,
                    numReadonlyUnsignedAccounts = numReadonlyUnsignedAccounts
                },
                accountKeys = accountKeys.ToArray(),
                recentBlockhash = recentBlockhash,
                instructions = instructions.ToArray()
            };
            return new Message(args);
        }
        private Message _compile()
        {
            var message = this.compileMessage();
            var signedKeys = message.accountKeys.Take(message.header.numRequiredSignatures);

            if (this.signatures.Count() == signedKeys.Count())
            {
                var valid = true;
                for(var i = 0; i < signatures.Count(); i++)
                {
                    if (!signedKeys.ElementAt(i).Equals(this.signatures[i]))
                    {
                        valid = false;
                        break;
                    }
                }
                //var valid = this.signatures(signature =>
                //{
                //    return signedKeys[signature.index].equals(signature.publicKey);
                //});
                if (valid) return message;
            }

            this.signatures = signedKeys.Select(publicKey => new SignaturePubkeyPair()
            {
                signature = null,
                publicKey = publicKey,
            }).ToList();
            return message;
        }

        ///**
        // * Get a buffer of the Transaction data that need to be covered by signatures
        // */
        //serializeMessage(): Buffer
        //{
        //    return this._compile().serialize();
        //}

        ///**
        // * Specify the public keys which will be used to sign the Transaction.
        // * The first signer will be used as the transaction fee payer account.
        // *
        // * Signatures can be added with either `partialSign` or `addSignature`
        // *
        // * @deprecated Deprecated since v0.84.0. Only the fee payer needs to be
        // * specified and it can be set in the Transaction constructor or with the
        // * `feePayer` property.
        // */
        //setSigners(...signers: Array<PublicKey>) {
        //    if (signers.length === 0)
        //    {
        //        throw new Error('No signers');
        //    }

        //    const seen = new Set();
        //    this.signatures = signers
        //      .filter(publicKey => {
        //          const key = publicKey.toString();
        //          if (seen.has(key))
        //          {
        //              return false;
        //          }
        //          else
        //          {
        //              seen.add(key);
        //              return true;
        //          }
        //      })
        //      .map(publicKey => ({ signature: null, publicKey}));
        //}


        ///**
        // * Partially sign a transaction with the specified accounts. All accounts must
        // * correspond to either the fee payer or a signer account in the transaction
        // * instructions.
        // *
        // * All the caveats from the `sign` method apply to `partialSign`
        // */
        //partialSign(...signers: Array<Account>) {
        //    if (signers.length === 0)
        //    {
        //        throw new Error('No signers');
        //    }

        //    // Dedupe signers
        //    const seen = new Set();
        //    const uniqueSigners = [];
        //    for (const signer of signers) {
        //        const key = signer.publicKey.toString();
        //        if (seen.has(key))
        //        {
        //            continue;
        //        }
        //        else
        //        {
        //            seen.add(key);
        //            uniqueSigners.push(signer);
        //        }
        //    }

        //    const message = this._compile();
        //    this._partialSign(message, ...uniqueSigners);
        //}

        /**
         * @internal
         */
        private void _partialSign(Message message, Account[] signers)
        {
            var signData = message.serialize();
            foreach(var signer in signers)
            {
                var signature = nacl.sign.detached(signData, signer.SecretKey);
                this._addSignature(signer.PublicKey, signature);
            }
            //signers.forEach(signer =>
            //{
                
            //});
        }

        ///**
        // * Add an externally created signature to a transaction. The public key
        // * must correspond to either the fee payer or a signer account in the transaction
        // * instructions.
        // */
        //addSignature(pubkey: PublicKey, signature: Buffer) {
        //    this._compile(); // Ensure signatures array is populated
        //    this._addSignature(pubkey, signature);
        //}

        /**
         * @internal
         */
        private void _addSignature(PublicKey pubkey, byte[] signature)
        {
            if(signature.Length != 64)
            {
                throw new Exception("signature length error");
            }
            //invariant(signature.Length == 64);
            var index = this.signatures.FindIndex(sigpair => pubkey.Equals(sigpair.publicKey));
            if (index < 0)
            {
                throw new Exception(string.Format("unknown signer: {0}", pubkey.ToString()));
            }
            var sign = this.signatures[index];
            sign.signature = signature;
            this.signatures[index] = sign;
        }

        ///**
        // * Verify signatures of a complete, signed Transaction
        // */
        //verifySignatures(): boolean
        //{
        //    return this._verifySignatures(this.serializeMessage(), true);
        //}

        /**
         * @internal
         */
        private bool _verifySignatures(byte[] signData, bool requireAllSignatures)
        {
            foreach (var sign in this.signatures)
            {

                var signature = sign.signature;
                var publicKey = sign.publicKey;

                if (signature == null)
                {
                    if (requireAllSignatures)
                    {
                        return false;
                    }
                }
                else
                {
                    if (!nacl.sign.detached.verify(signData, signature, publicKey.toBuffer()))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

    ///**
    // * Serialize the Transaction in the wire format.
    // */
    //serialize(config ?: SerializeConfig): Buffer
    //{
    //    const { requireAllSignatures, verifySignatures} = Object.assign(
    //      { requireAllSignatures: true, verifySignatures: true},
    //      config,
    //    );

    //    const signData = this.serializeMessage();
    //    if (
    //      verifySignatures &&
    //      !this._verifySignatures(signData, requireAllSignatures)
    //    )
    //    {
    //        throw new Error('Signature verification failed');
    //    }

    //    return this._serialize(signData);
    //}

    ///**
    // * @internal
    // */
    //_serialize(signData: Buffer): Buffer
    //{
    //    const { signatures} = this;
    //    const signatureCount: number[] = [];
    //    shortvec.encodeLength(signatureCount, signatures.length);
    //    const transactionLength =
    //      signatureCount.length + signatures.length * 64 + signData.length;
    //    const wireTransaction = Buffer.alloc(transactionLength);
    //    invariant(signatures.length < 256);
    //    Buffer.from(signatureCount).copy(wireTransaction, 0);
    //    signatures.forEach(({ signature}, index) => {
    //        if (signature !== null)
    //        {
    //            invariant(signature.length === 64, `signature has invalid length`);
    //            Buffer.from(signature).copy(
    //              wireTransaction,
    //              signatureCount.length + index * 64,

    //            );
    //        }
    //    });
    //    signData.copy(
    //      wireTransaction,
    //      signatureCount.length + signatures.length * 64,
    //    );
    //    invariant(
    //      wireTransaction.length <= PACKET_DATA_SIZE,
    //      `Transaction too large: ${ wireTransaction.length} > ${ PACKET_DATA_SIZE}`,
    //    );
    //    return wireTransaction;
    //}

    ///**
    // * Deprecated method
    // * @internal
    // */
    //get keys(): Array<PublicKey> {
    //    invariant(this.instructions.length === 1);
    //    return this.instructions[0].keys.map(keyObj => keyObj.pubkey);
    //}

    ///**
    // * Deprecated method
    // * @internal
    // */
    //get programId(): PublicKey
    //{
    //    invariant(this.instructions.length === 1);
    //    return this.instructions[0].programId;
    //}

    ///**
    // * Deprecated method
    // * @internal
    // */
    //get data(): Buffer
    //{
    //    invariant(this.instructions.length === 1);
    //    return this.instructions[0].data;
    //}

    ///**
    // * Parse a wire transaction into a Transaction object.
    // */
    //static from(buffer: Buffer | Uint8Array | Array<number>): Transaction
    //{
    //    // Slice up wire data
    //    let byteArray = [...buffer];

    //    const signatureCount = shortvec.decodeLength(byteArray);
    //    let signatures = [];
    //    for (let i = 0; i < signatureCount; i++)
    //    {
    //        const signature = byteArray.slice(0, SIGNATURE_LENGTH);
    //        byteArray = byteArray.slice(SIGNATURE_LENGTH);
    //        signatures.push(bs58.encode(Buffer.from(signature)));
    //    }

    //    return Transaction.populate(Message.from(byteArray), signatures);
    //}

    ///**
    // * Populate Transaction object from message and signatures
    // */
    //static populate(message: Message, signatures: Array<string>): Transaction
    //{
    //    const transaction = new Transaction();
    //    transaction.recentBlockhash = message.recentBlockhash;
    //    if (message.header.numRequiredSignatures > 0)
    //    {
    //        transaction.feePayer = message.accountKeys[0];
    //    }
    //    signatures.forEach((signature, index) => {
    //        const sigPubkeyPair = {
    //        signature:
    //          signature == bs58.encode(DEFAULT_SIGNATURE)
    //            ? null
    //            : bs58.decode(signature),
    //        publicKey: message.accountKeys[index],
    //      };
    //    transaction.signatures.push(sigPubkeyPair);
    //});

    //message.instructions.forEach(instruction => {
    //const keys = instruction.accounts.map(account => {
    //    const pubkey = message.accountKeys[account];
    //    return {
    //        pubkey,
    //          isSigner: transaction.signatures.some(
    //            keyObj => keyObj.publicKey.toString() === pubkey.toString(),
    //          ),
    //          isWritable: message.isAccountWritable(account),
    //        };
    //});

    //transaction.instructions.push(
    //  new TransactionInstruction({
    //    keys,
    //          programId: message.accountKeys[instruction.programIdIndex],
    //          data: bs58.decode(instruction.data),
    //  }),

    //  );
    //    });

    //return transaction;
    //  }






}
}
