using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Solana.Util;
using NokitaKaze.Base58Check;

namespace Solana
{



    /// <summary>
    /// The message header, identifying signed and read-only account
    /// </summary>
    public struct MessageHeader
    {
        /// <summary>
        /// The number of signatures required for this message to be considered valid. The
        /// signatures must match the first `numRequiredSignatures` of `accountKeys`.
        /// </summary>
        public int numRequiredSignatures;
        /// <summary>
        /// The last `numReadonlySignedAccounts` of the signed keys are read-only accounts
        /// </summary>
        public int numReadonlySignedAccounts;
        /// <summary>
        /// The last `numReadonlySignedAccounts` of the unsigned keys are read-only accounts
        /// </summary>
        public int numReadonlyUnsignedAccounts;
    }

    /// <summary>
    /// An instruction to execute by a program
    /// </summary>
    public struct CompiledInstruction
    {
        /// <summary>
        /// Index into the transaction keys array indicating the program account that executes this instruction
        /// </summary>
        public int programIdIndex;
        /// <summary>
        /// Ordered indices into the transaction keys array indicating which accounts to pass to the program
        /// </summary>
        public int[] accounts;
        /// <summary>
        /// The program input data encoded as base 58
        /// </summary>
        public string data;
    }


    /// <summary>
    /// Message constructor arguments
    /// </summary>
    public struct MessageArgs
    {
        /// <summary>
        /// The message header, identifying signed and read-only `accountKeys`
        /// </summary>
        public MessageHeader header;
        /// <summary>
        /// All the account keys used by this transaction
        /// </summary>
        public string[] accountKeys;
        /// <summary>
        /// The hash of a recent ledger block
        /// </summary>
        public string recentBlockhash;
        /// <summary>
        /// Instructions that will be executed in sequence and committed in one atomic transaction if all succeed.
        /// </summary>
        public CompiledInstruction[] instructions;
    }


    /// <summary>
    /// List of instructions to be processed atomically
    /// </summary>
    public class Message
    {
        public MessageHeader header;
        public PublicKey[] accountKeys;
        string recentBlockhash;
        CompiledInstruction[] instructions;

        public Message(MessageArgs args)
        {
            this.header = args.header;
            this.accountKeys = args.accountKeys.ToList().Select(account => new PublicKey(account)).ToArray();
            this.recentBlockhash = args.recentBlockhash;
            this.instructions = args.instructions;
        }

        private bool isAccountWritable(ulong index)
        {
            return (
              index < header.numRequiredSignatures - header.numReadonlySignedAccounts ||
              (index >= header.numRequiredSignatures && index < Convert.ToUInt64(accountKeys.Length) - header.numReadonlyUnsignedAccounts)
            );
        }

        public byte[] serialize()
        {
            return new byte[0];
            //var numKeys = this.accountKeys.Length;

            //    ulong[] keyCount = ShortVec.encodeLength(numKeys);

            //    var instructions = this.instructions.ToList().Select(instruction =>
            //    {
            //    //const { accounts, programIdIndex} = instruction;
            //    var accounts = instruction.accounts;
            //    var programIdIndex = instruction.programIdIndex;
            //    var data = Base58CheckEncoding.DecodePlain(instruction.data);

            //    var keyIndicesCount = ShortVec.encodeLength(accounts.Length);

            //    var dataCount = ShortVec.encodeLength(data.Length);

            //    return {
            //        programIdIndex,
            //    keyIndicesCount: Buffer.from(keyIndicesCount),
            //    keyIndices: Buffer.from(accounts),
            //    dataLength: Buffer.from(dataCount),
            //    data,
            //  };
            //});

            //    ulong[] instructionCount = ShortVec.encodeLength(instructions.length);
            //    var instructionBuffer = Buffer.alloc(PACKET_DATA_SIZE);
            //    Buffer.from(instructionCount).copy(instructionBuffer);
            //    let instructionBufferLength = instructionCount.length;

            //    instructions.forEach(instruction =>
            //    {
            //    const instructionLayout = BufferLayout.struct([
            //      BufferLayout.u8('programIdIndex'),

            //      BufferLayout.blob(
            //        instruction.keyIndicesCount.length,
            //        'keyIndicesCount',

            //      ),
            //      BufferLayout.seq(
            //        BufferLayout.u8('keyIndex'),
            //        instruction.keyIndices.length,
            //        'keyIndices',

            //      ),
            //      BufferLayout.blob(instruction.dataLength.length, 'dataLength'),
            //      BufferLayout.seq(
            //        BufferLayout.u8('userdatum'),
            //        instruction.data.length,
            //        'data',

            //      ),

            //    ]);
            //          const length = instructionLayout.encode(
            //            instruction,
            //            instructionBuffer,
            //            instructionBufferLength,
            //          );
            //instructionBufferLength += length;
            //        });
            //instructionBuffer = instructionBuffer.slice(0, instructionBufferLength);

            //const signDataLayout = BufferLayout.struct([
            //  BufferLayout.blob(1, 'numRequiredSignatures'),
            //  BufferLayout.blob(1, 'numReadonlySignedAccounts'),
            //  BufferLayout.blob(1, 'numReadonlyUnsignedAccounts'),
            //  BufferLayout.blob(keyCount.length, 'keyCount'),
            //  BufferLayout.seq(Layout.publicKey('key'), numKeys, 'keys'),
            //  Layout.publicKey('recentBlockhash'),

            //]);

            //const transaction = {
            //      numRequiredSignatures: Buffer.from([this.header.numRequiredSignatures]),
            //      numReadonlySignedAccounts: Buffer.from([
            //        this.header.numReadonlySignedAccounts,
            //      ]),
            //      numReadonlyUnsignedAccounts: Buffer.from([
            //        this.header.numReadonlyUnsignedAccounts,
            //      ]),
            //      keyCount: Buffer.from(keyCount),
            //      keys: this.accountKeys.map(key => toBuffer(key.toBytes())),
            //      recentBlockhash: bs58.decode(this.recentBlockhash),
            //    };

            //let signData = Buffer.alloc(2048);
            //const length = signDataLayout.encode(transaction, signData);
            //instructionBuffer.copy(signData, length);
            //return signData.slice(0, length + instructionBuffer.length);
        }

        //  /**
        //   * Decode a compiled message into a Message object.
        //   */
        //  static from(buffer: Buffer | Uint8Array | Array<number>): Message
        //{
        //    // Slice up wire data
        //    let byteArray = [...buffer];

        //    const numRequiredSignatures = byteArray.shift() as number;
        //    const numReadonlySignedAccounts = byteArray.shift() as number;
        //    const numReadonlyUnsignedAccounts = byteArray.shift() as number;

        //    const accountCount = shortvec.decodeLength(byteArray);
        //    let accountKeys = [];
        //    for (let i = 0; i < accountCount; i++)
        //    {
        //        const account = byteArray.slice(0, PUBKEY_LENGTH);
        //        byteArray = byteArray.slice(PUBKEY_LENGTH);
        //        accountKeys.push(bs58.encode(Buffer.from(account)));
        //    }

        //    const recentBlockhash = byteArray.slice(0, PUBKEY_LENGTH);
        //    byteArray = byteArray.slice(PUBKEY_LENGTH);

        //    const instructionCount = shortvec.decodeLength(byteArray);
        //    let instructions: CompiledInstruction[] = [];
        //    for (let i = 0; i < instructionCount; i++)
        //    {
        //        const programIdIndex = byteArray.shift() as number;
        //        const accountCount = shortvec.decodeLength(byteArray);
        //        const accounts = byteArray.slice(0, accountCount);
        //        byteArray = byteArray.slice(accountCount);
        //        const dataLength = shortvec.decodeLength(byteArray);
        //        const dataSlice = byteArray.slice(0, dataLength);
        //        const data = bs58.encode(Buffer.from(dataSlice));
        //        byteArray = byteArray.slice(dataLength);
        //        instructions.push({
        //            programIdIndex,
        //        accounts,
        //        data,
        //      });
        //}

        //const messageArgs = {
        //      header: {
        //    numRequiredSignatures,
        //        numReadonlySignedAccounts,
        //        numReadonlyUnsignedAccounts,
        //      },
        //      recentBlockhash: bs58.encode(Buffer.from(recentBlockhash)),
        //      accountKeys,
        //      instructions,
        //    };

        //return new Message(messageArgs);
        //  }
    }
}
